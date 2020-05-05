using Android.App;
using Android.Runtime;
using System;

namespace ShinyBleApp.Droid
{
    [Application]
    public class MainApplication : Shiny.ShinyAndroidApplication<ShinyAppStartup>
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }
    }
}