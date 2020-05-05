using Shiny.BluetoothLE;
using Shiny.BluetoothLE.Central;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShinyBleApp.Models.Items
{
    public class Device : IDisposable
    {
        IDisposable _statusDisposable;
        IDisposable _readDisposable;
        IDisposable _connectionWaitDisposable;


        public Device(IPeripheral peripheral)
        {
            Peripheral = peripheral;
        }
        ~Device()
        {

        }

        #region Variable

        #region Public
        public string Name => Peripheral.Name;
        public Guid Id => Peripheral.Uuid;
        public ConnectionState PrevStatus { get; set; } = default;
        public ConnectionState State => Peripheral.Status;
        #endregion

        #region Private
        IPeripheral Peripheral { get; set; }
        IGattCharacteristic ReadGattChar { get; set; }
        IGattCharacteristic WriteGattChar { get; set; }
        GattState GattStatus { get; set; } = GattState.None;

        
        bool IsSubscribeStatus { get; set; } = false;
        bool IsSubscribeConnetionWait { get; set; } = false;
        #endregion

        #endregion

        public void Dispose()
        {
            Peripheral?.CancelConnection();
            _statusDisposable?.Dispose();
            _readDisposable?.Dispose();
            _connectionWaitDisposable?.Dispose();

            _statusDisposable = default;
            _readDisposable = default;
            _connectionWaitDisposable = default;

            IsSubscribeStatus = false;
            IsSubscribeConnetionWait= false;
        }

        public void ConnectionWait(bool auto = true)
        {
            App.AddLog($"{Peripheral.Name} ConnectionWait : {Helper.GetEnumName(State)}");
            SubcribeStatus();
            MessagingCenter.Send(this, "StateChanged", State);
            SubcribeConnectionWait(auto);
        }

        public void CancelConnection()
        {
            App.AddLog($"{Peripheral.Name} CancelConnect : {Helper.GetEnumName(State)}");
            _readDisposable?.Dispose();
            _connectionWaitDisposable?.Dispose();

            _connectionWaitDisposable = default;
            _readDisposable = default;

            Peripheral?.CancelConnection();
        }

        public void GetGatt()
        {
            if (GattStatus == GattState.Allow
               || GattStatus == GattState.Progress)
                return;

            GattStatus = GattState.Progress;
            try
            {
                App.AddLog($"GetGatt Waiting");
                Task.Delay(Constants.Ble.OperationTimeout).Wait();
                App.AddLog($"GetGatt Start");
                var service = Peripheral.GetKnownService(Constants.Ble.ServiceGuid)
                    .Timeout(Constants.Ble.OperationTimeout)
                    .Wait();
                ReadGattChar = service.GetKnownCharacteristics(Constants.Ble.ReadGattCharGuid)
                    .Timeout(Constants.Ble.OperationTimeout)
                    .Wait();
                WriteGattChar = service.GetKnownCharacteristics(Constants.Ble.WriteGattCharGuid)
                    .Timeout(Constants.Ble.OperationTimeout)
                    .Wait();

                _readDisposable = ReadGattChar.Notify().Subscribe(OnReceive);

                GattStatus = GattState.Allow;
                MessagingCenter.Send(this, "GetGatt", true);
            }
            catch (Exception ex)
            {
                App.AddLog($"GetGatt {ex.Message}");
                GattStatus = GattState.None;
                MessagingCenter.Send(this, "GetGatt", false);
            }
        }


        public async Task<bool> SendAsync(string data)
        {
            if (GattStatus == GattState.Allow)
            {
                try
                {
                    var ret = await WriteGattChar.Write(Encoding.UTF8.GetBytes(data));
                    App.AddLog($"{MethodBase.GetCurrentMethod().Name} {Name} {data} ");
                    return true;
                }
                catch (Exception ex)
                {
                    App.AddLog($"{MethodBase.GetCurrentMethod().Name} {Name} {data} {ex.Message} ");
                    return false;
                }
            }
            else
                return false;
        }

        void OnReceive(CharacteristicGattResult result)
        {
            string data;
            try
            {
                data = Encoding.UTF8.GetString(result.Data);
            }
            catch (Exception ex) { App.AddLog($"{MethodBase.GetCurrentMethod().Name} {Name} {ex.Message}"); return; }

            MessagingCenter.Send(this, "Received", data);
        }


        #region Subscribe
        #region ConenctionWait
        void SubcribeConnectionWait(bool auto)
        {
            if (!IsSubscribeConnetionWait)
            {
                IsSubscribeConnetionWait = true;
                try
                {
                    _connectionWaitDisposable = Peripheral.ConnectWait(auto ? Constants.Ble.AutoConnectionConfig : Constants.Ble.ConnectionConfig)
                        .Subscribe(
                            OnConnectionWait,
                            ex => OnError(ex, ObserverType.ConnectionWait),
                            () => OnComplete(ObserverType.ConnectionWait));
                }
                catch (Exception ex) 
                {
                    App.AddLog($"{MethodBase.GetCurrentMethod().Name} {Name} {ex.Message}");
                    IsSubscribeConnetionWait = false;
                    _connectionWaitDisposable?.Dispose();
                    _connectionWaitDisposable = default;
                }
            }
        }
        void OnConnectionWait(IPeripheral peripheral)
        {
            // Not to do
        }

        #endregion

        #region Status
        void SubcribeStatus()
        {
            if (!IsSubscribeStatus)
            {
                IsSubscribeStatus = true;
                try
                {
                    _statusDisposable = Peripheral.WhenStatusChanged()
                        .Subscribe(
                            OnStatus,
                            ex => OnError(ex, ObserverType.State),
                            () => OnComplete(ObserverType.State));
                }catch(Exception ex)
                {
                    App.AddLog($"{MethodBase.GetCurrentMethod().Name} {Name} {ex.Message}");
                    IsSubscribeStatus = false;
                }
            }
        }
        void OnStatus(ConnectionState status)
        {
            switch (status)
            {
                case ConnectionState.Connected: break;
                case ConnectionState.Connecting: break;
                case ConnectionState.Disconnected:
                    {
                        GattStatus = GattState.None;
                        _readDisposable?.Dispose();
                        _readDisposable = default;
                        break;
                    }
                case ConnectionState.Disconnecting: break;
            }

            MessagingCenter.Send(this, "StatusChanged", status);
        }
        #endregion

        #region Shared
        private void OnError(Exception ex, ObserverType type)
        {
            App.AddLog($"{MethodBase.GetCurrentMethod().Name} {Name} {Helper.GetEnumName(type)} {ex.Message}");

            if (type == ObserverType.State)
            {
                IsSubscribeStatus = false;
            }
            else if (type == ObserverType.ConnectionWait)
            {
                IsSubscribeConnetionWait = false;
                _connectionWaitDisposable?.Dispose();
                _connectionWaitDisposable = default;
            }
        }

        private void OnComplete(ObserverType type)
        {
            App.AddLog($"{MethodBase.GetCurrentMethod().Name} {Name} {Helper.GetEnumName(type)}");

            if (type == ObserverType.State)
            {
                IsSubscribeStatus = false;
            }
            else if (type == ObserverType.ConnectionWait)
            {
                IsSubscribeConnetionWait = false;
                _connectionWaitDisposable?.Dispose();
                _connectionWaitDisposable = default;
            }
        }
        #endregion

        #endregion


        enum GattState
        {
            None, Allow, Progress
        }

        enum ObserverType
        {
            State, ConnectionWait
        }
    }
}
