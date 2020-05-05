using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;

namespace ShinyBleApp.Droid.Receivers
{
    [BroadcastReceiver]
    public class ScreenOnReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (!string.IsNullOrEmpty(intent.Action))
            {
                App.AddLog(intent.Action);
                if (intent.Action == Intent.ActionScreenOff)
                {
                    App.AddLog("ActionScreenOff");
                    MessagingCenter.Send(this, "ScreenOff");

                }
                if (intent.Action == Intent.ActionScreenOn)
                {
                    App.AddLog("ActionScreenOn");
                    MessagingCenter.Send(this, "ScreenOn");
                }
            }
        }
    }
}