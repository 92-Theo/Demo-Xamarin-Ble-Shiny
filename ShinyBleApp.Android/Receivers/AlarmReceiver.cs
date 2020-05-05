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

namespace ShinyBleApp.Droid.Receivers
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            App.AddLog("Alarm Receive");
            //var powerManager = (PowerManager)context.GetSystemService(Context.PowerService);
            //var wakeLock = powerManager.NewWakeLock(WakeLockFlags.Partial, context.PackageName);
            //if (!powerManager.IsInteractive)
            //{
            //    if (wakeLock.IsHeld)
            //        wakeLock.Release();
            //}
            //AndroidNotificationManager.Instance.Notify("알람", $"{DateTime.Now}", "");
        }
    }
}