using MonkeyCache.FileStore;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShinyBleApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Views.DeviceListPage();

            Barrel.ApplicationId = "ShinyBleApp_v2";

            _ = Models.Managers.BleManager.Instance;
            _ = Models.Managers.PermissionManager.Instance;
            _ = Models.Managers.StatusManager.Instance;
            _ = Models.Managers.TimerManager.Instance;

            _ = ViewModels.DeviceListViewModel.Instance;
            //_ = Models.Managers.ViewManager.Instance;
        }

        protected override void OnStart()
        {
            Models.Managers.PermissionManager.Instance.RequestLocation();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public static void AddLog(string msg)
        {
            string msg2 = $"[{DateTime.Now.ToLongTimeString()}]{msg}";
            ViewModels.DeviceListViewModel.Instance.AddLog(msg2);
            Debug.WriteLine(msg2);
        }
    }
}
