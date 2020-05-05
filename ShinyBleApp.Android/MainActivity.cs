using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using ShinyBleApp.Droid.Services;
using Plugin.Permissions;

namespace ShinyBleApp.Droid
{
    [Activity(Label = "ShinyBleApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());


            Xamarin.Forms.MessagingCenter.Subscribe<ViewModels.DeviceListViewModel>(this, "StartLongRunningService", s =>
            {
                var intent = new Intent(this, typeof(LongRunningTaskService));
                if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    StartForegroundService(intent);
                }
                else
                {
                    StartService(intent);
                }
            });

            Xamarin.Forms.MessagingCenter.Subscribe<ViewModels.DeviceListViewModel>(this, "StopLongRunningService", s =>
            {
                var intent = new Intent(this, typeof(LongRunningTaskService));
                StopService(intent);
            });
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Shiny.AndroidShinyHost.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}