using MonkeyCache.FileStore;
using Shiny.BluetoothLE.Central;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ShinyBleApp.Models.Managers
{
    public class StatusManager
    {
        static Lazy<StatusManager> _lazy = new Lazy<StatusManager>(() => new StatusManager());
        public static StatusManager Instance { get => _lazy.Value; }


        StatusManager()
        {
            if (Barrel.Current.Exists("WhiteList"))
                WhiteList = Barrel.Current.Get<ObservableCollection<string>>("WhiteList");
            else
                WhiteList = new ObservableCollection<string>();
            WhiteList.CollectionChanged += WhiteList_CollectionChanged;
        }


        public ObservableCollection<string> WhiteList { get; private set; }
        public bool IsBleOn => BleManager.Instance.Status == Shiny.AccessState.Available;


        void WhiteList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateLocalData("WhiteList", WhiteList);
        }

        public void UpdateLocalData<T> (string key, T data)
        {
            if (Barrel.Current.Exists(key))
            {
                Barrel.Current.Empty(key);
            }
            
            Barrel.Current.Add<T>(key, data, TimeSpan.MaxValue);
        }
    }
}
