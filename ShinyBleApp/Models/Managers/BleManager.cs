using Shiny;
using Shiny.BluetoothLE;
using Shiny.BluetoothLE.Central;
using ShinyBleApp.Models.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShinyBleApp.Models.Managers
{
    public class BleManager
    {
        static Lazy<BleManager> _lazy = new Lazy<BleManager>(() => new BleManager());
        public static BleManager Instance { get => _lazy.Value; }


        BleManager()
        {
            SubscribeMssagingCenter();
            CentralManager.WhenStatusChanged().Subscribe(
                OnCentralStatusChanged,
                OnCentralStatusError,
                OnCentralStatusComplete);



            Task.Factory.StartNew(() =>
            {
                foreach (var id in StatusManager.Instance.WhiteList)
                {
                    var peripheral = CentralManager.GetKnownPeripheral(Guid.Parse(id)).Wait();
                    AddDevice(peripheral);
                    Task.Factory.StartNew(() => { DeviceDic[id].ConnectionWait(); });
                }
            });
        }


        IDisposable _scanDisposable;
        ICentralManager CentralManager => Shiny.ShinyBLE.Central;
        bool IsScanning { get; set; }
        Dictionary<string, Items.Device> DeviceDic { get; set; } = new Dictionary<string, Items.Device>();
        ObservableCollection<string> WhiteList => StatusManager.Instance.WhiteList;

        public AccessState Status => CentralManager.Status;


        public void AddDevice(IPeripheral peripheral)
        {
            if (!DeviceDic.ContainsKey(peripheral.Uuid.ToString()))
            {
                App.AddLog($"{MethodBase.GetCurrentMethod().Name} {peripheral.Uuid}");
                DeviceDic.Add(peripheral.Uuid.ToString(), new Items.Device(peripheral));
                MessagingCenter.Send(this, "DeviceAdded", new DeivceView() { Id = peripheral.Uuid.ToString(), Status = Helper.GetEnumName(peripheral.Status) });
            }
        }

        public void RemoveDevice(string id)
        {
            if (DeviceDic.ContainsKey(id))
            {
                DeviceDic[id].Dispose();
                DeviceDic.Remove(id);
            }
        }

        #region Connection

        public void Connect(string id)
        {
            if (!WhiteList.Contains(id))
            {
                WhiteList.Add(id);
            }

            Task.Run(() =>
            {
                var peripheral = CentralManager.GetKnownPeripheral(Guid.Parse(id)).Timeout(Constants.Ble.GetKnownPeripheralTimeout).Wait();
                if (peripheral != default)
                {
                    AddDevice(peripheral);
                }

                Task.Factory.StartNew(() => DeviceDic[id]?.ConnectionWait());
            });
        }
        
        public void Disconnect(string id)
        {
            if (WhiteList.Contains(id))
            {
                WhiteList.Remove(id);
            }

            DeviceDic[id]?.CancelConnection();
        }

        #endregion


        #region Scan
        public async Task StartScan(TimeSpan timeout)
        {
            try
            {
                _scanDisposable = CentralManager
                    .ScanForUniquePeripherals() //Constants.Ble.ScanConfig
                    .Subscribe(
                        device =>
                        {
                            AddDevice(device);
                        },
                        ex => App.AddLog(ex.Message)
                    );
            }
            catch (Exception) { }

            IsScanning = true;
            DateTime startingTime = DateTime.Now;
            App.AddLog($"Delay {startingTime.ToLongTimeString()}");
            while ((DateTime.Now - startingTime) < timeout)
            {
                await Task.Delay(1);
            }

            await StopScan();
        }
        public async Task StopScan()
        {
            if (IsScanning)
            {
                App.AddLog("Stop Scan");
                IsScanning = false;
                _scanDisposable?.Dispose();
                _scanDisposable = default;
                await Task.Delay(100);
            }
        }

        #endregion


        void SubscribeMssagingCenter()
        {

            MessagingCenter.Subscribe<Items.Device, ConnectionState>(this, "StatusChanged", (s, e) =>
            {
                App.AddLog($"{s.Name} StatusChanged {Helper.GetEnumName(e)}");
                if (s.PrevStatus == e)
                {
                    App.AddLog($"{s.Name} StatusChanged Same with PrevStatus");
                }
                else
                {
                    s.PrevStatus = e;
                    switch (e)
                    {
                        case ConnectionState.Connected: OnConnected(s); break;
                        case ConnectionState.Disconnected: OnDisconnected(s); break;
                    }

                    MessagingCenter.Send(this, "DeviceStatusChanged", new DeivceView() { Id = s.Id.ToString(), Status = Helper.GetEnumName(e) });
                }
            });
     

            MessagingCenter.Subscribe<Items.Device, bool>(this, "GetGatt", (s, e) =>
            {
                App.AddLog($"{s.Name} GetGatt {e}");
            });

            MessagingCenter.Subscribe<Items.Device, string>(this, "Received", (s, e) =>
            {
                App.AddLog($"{s.Name} Received {e}");
            });
        }
        void OnConnected(Items.Device device)
        {
            if (!WhiteList.Contains(device.Id.ToString()))
                device.CancelConnection();
        }
        void OnDisconnected(Items.Device device)
        {
            if (WhiteList.Contains(device.Id.ToString()))
                Task.Factory.StartNew(() => device.ConnectionWait()); ;
        }

        void OnCentralStatusChanged(AccessState status)
        {
            App.AddLog($"{MethodBase.GetCurrentMethod().Name} {Helper.GetEnumName(status)}");
        }

        void OnCentralStatusError(Exception ex)
        {
            App.AddLog($"{MethodBase.GetCurrentMethod().Name} {ex.Message}");
        }
        void OnCentralStatusComplete()
        {
            App.AddLog($"{MethodBase.GetCurrentMethod().Name}");
        }
    }
}
