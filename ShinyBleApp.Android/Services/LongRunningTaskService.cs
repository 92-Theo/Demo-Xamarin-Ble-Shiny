using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using ShinyBleApp.Droid.Receivers;
using ShinyBleApp.Models.Managers;

namespace ShinyBleApp.Droid.Services
{
    [Service]
    public class LongRunningTaskService : Service
    {
        bool IsStarted { get; set; }
        ScreenOnReceiver ScreenOnReceiver { get; set; }
        string CurGuid { get; set; } = "";

        
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            if (Build.VERSION.SdkInt > BuildVersionCodes.O) //안드로이드버전이 높으면
            {
                var channel = new NotificationChannel(Constants.Notification.ChannelId, Constants.Notification.ChannelName, NotificationImportance.Default);
                channel.EnableVibration(false);
                channel.SetShowBadge(false);
                channel.SetVibrationPattern(default);
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (!IsStarted)
            {
                IsStarted = true;
                RegisterForegroundService();
                Models.Managers.TimerManager.Instance.ScanTimer.Start();
                ScreenOnReceiver = new ScreenOnReceiver();
                var filter = new IntentFilter();
                filter.AddAction(Intent.ActionScreenOn);
                filter.AddAction(Intent.ActionScreenOff);
                RegisterReceiver(ScreenOnReceiver, filter);
                Xamarin.Forms.MessagingCenter.Subscribe<ScreenOnReceiver>(this, "ScreenOff", s =>
                {
                    StartAlarm();
                });
                Xamarin.Forms.MessagingCenter.Subscribe<ScreenOnReceiver>(this, "ScreenOn", s =>
                {
                    StopAlarm();
                });
                
                Xamarin.Forms.MessagingCenter.Subscribe<BleManager, Models.Items.DeivceView>(this, "DeviceStatusChanged", (s, e) =>
                {
                    UpdateNotificationContext($"{e.Id} {e.Status}");
                });

            }
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            //if (_cts != null)
            //{
            //    _cts.Token.ThrowIfCancellationRequested();
            //    _cts.Cancel();
            //}
            base.OnDestroy();

            if (IsStarted)
            {
                CurGuid = default;
                Models.Managers.TimerManager.Instance.ScanTimer.Stop();
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.Cancel(Constants.Notification.Id);
                UnregisterReceiver(ScreenOnReceiver);
                Xamarin.Forms.MessagingCenter.Unsubscribe<ScreenOnReceiver>(this, "ScreenOff");
                Xamarin.Forms.MessagingCenter.Unsubscribe<ScreenOnReceiver>(this, "ScreenOn");
                Xamarin.Forms.MessagingCenter.Unsubscribe<BleManager, ShinyBleApp.Models.Items.Device>(this, "Connected");
                IsStarted = false;
            }
        }


        private void RegisterForegroundService()
        {
            var notification = GetNotificationCompat("-");
            StartForeground(Constants.Notification.Id, notification.Build());
        }

        private void UpdateNotificationContext(string guid)
        {
            if (CurGuid == guid)
                return;

            var notification = GetNotificationCompat(guid);
            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.Notify(Constants.Notification.Id, notification.Build());
        }

        private NotificationCompat.Builder GetNotificationCompat(string guid)
        {
            CurGuid = guid;

            var notification = new NotificationCompat.Builder(Application.Context, Constants.Notification.ChannelId)
               .SetSmallIcon(Resource.Drawable.notification_icon_background)
               .SetVibrate(null)
               .SetContentIntent(BuildIntentToShowMainActivity())
               .SetOngoing(true)
               .SetContentText(guid)
               .SetPriority(NotificationCompat.PriorityHigh);

            return notification;
        }

        private PendingIntent BuildIntentToShowMainActivity()
        {
            var notificationIntent = new Intent(Application.Context, typeof(MainActivity));
            //notificationIntent.SetAction(Constants.ACTION_MAIN_ACTIVITY);
            notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
            //notificationIntent.PutExtra(Constants.SERVICE_STARTED_KEY, true);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }


        void StartAlarm()
        {
            AlarmManager manager = (AlarmManager)GetSystemService(AlarmService);
            long triggerAtTime = SystemClock.ElapsedRealtime() + (10 * 60 * 1000);
            Intent alarmintent = new Intent(this, typeof(AlarmReceiver));

            PendingIntent pendingintent = PendingIntent.GetBroadcast(this, 0, alarmintent, 0);
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                manager.Cancel(pendingintent);
                manager.SetAndAllowWhileIdle(AlarmType.ElapsedRealtimeWakeup, triggerAtTime, pendingintent);
                App.AddLog("Alarm SetAndAllowWhileIdle Set");
            }
        }

        void StopAlarm()
        {
            AlarmManager manager = (AlarmManager)GetSystemService(AlarmService);
            Intent alarmintent = new Intent(this, typeof(AlarmReceiver));
            PendingIntent pendingintent = PendingIntent.GetBroadcast(this, 0, alarmintent, 0);
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                manager.Cancel(pendingintent);
            }
        }
    }
}