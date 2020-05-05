using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ShinyBleApp.Models.Items;
using ShinyBleApp.Models.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace ShinyBleApp.ViewModels
{
    public class DeviceListViewModel : ViewModelBase
    {
        static Lazy<DeviceListViewModel> _lazy = new Lazy<DeviceListViewModel>(() => new DeviceListViewModel());
        public static DeviceListViewModel Instance { get => _lazy.Value; }

        DeviceListViewModel()
        {
            Connect = new RelayCommand(OnConnect);
            Disconnect = new RelayCommand(OnDisconnect);
            Start = new RelayCommand(OnStart);
            Stop = new RelayCommand(OnStop);

            MessagingCenter.Subscribe<BleManager, DeivceView>(this, "DeviceStatusChanged", (s,e)=>
            {
                
                ViewManager.InvokeOnMainThread(() =>
                {
                    var device = DeviceList.FirstOrDefault(x => x.Id == e.Id);
                    if (device != default)
                    {
                        App.AddLog($"DeviceStatusChanged {device.Id} {e.Status}");
                        try
                        {
                            DeviceList.Remove(device);
                            DeviceList.Insert(0, e);
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        App.AddLog($"DeviceStatusChanged Not {e.Id} {e.Status}");
                    }
                });
            });

            MessagingCenter.Subscribe<BleManager, DeivceView>(this, "DeviceAdded", (s, e) =>
            {
                ViewManager.InvokeOnMainThread(() =>
                {
                    try
                    {
                        DeviceList.Add(e);
                    }
                    catch (Exception) { }
                });
            });
        }


        private ObservableCollection<string> _logList = new ObservableCollection<string>();
        public ObservableCollection<string> LogList
        {
            get => _logList;
            set { _logList = value; RaisePropertyChanged("LogList"); }
        }

        private ObservableCollection<DeivceView> _DeviceList = new ObservableCollection<DeivceView>();
        public ObservableCollection<DeivceView> DeviceList
        {
            get => _DeviceList;
            set { _DeviceList = value; RaisePropertyChanged("DeviceList"); }
        }

        public DeivceView SelectedDevice { get; set; }
        public RelayCommand Connect { get; private set; }

        void OnConnect()
        {
            if (SelectedDevice != default)
            {
                BleManager.Instance.Connect(SelectedDevice.Id);
            }
            else
            {
                App.AddLog("SelectedDevice is default");
            }
        }

        public RelayCommand Disconnect { get; private set; }
        void OnDisconnect()
        {
            if (SelectedDevice != default)
            {
                BleManager.Instance.Disconnect(SelectedDevice.Id);
            }
            else
            {
                App.AddLog("SelectedDevice is default");
            }
        }


        public RelayCommand Start { get; private set; }
        void OnStart()
        {
            MessagingCenter.Send(this, "StartLongRunningService");
        }

        public RelayCommand Stop { get; private set; }
        void OnStop()
        {
            MessagingCenter.Send(this, "StopLongRunningService");
        }


        public void AddLog(string msg)
        {
            ViewManager.InvokeOnMainThread(() =>
            {
                try
                {
                    LogList.Add(msg);
                }
                catch (Exception) { }
            });
        }
    }
}
