using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ShinyBleApp.Droid.Receivers
{
    [BroadcastReceiver]
    public class BtReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            App.AddLog($"{MethodBase.GetCurrentMethod().Name} {intent.Action}");
            switch (intent.Action)
            {
                case BluetoothAdapter.ActionStateChanged: //블루투스의 연결 상태 변경
                    int state = intent.GetIntExtra(BluetoothAdapter.ExtraState, BluetoothAdapter.Error);
                    State outState;
                    if (Enum.TryParse(state.ToString(), out outState))
                    {
                        App.AddLog($"BtStateReceiver {Helper.GetEnumName(outState)}");
                        //switch (outState)
                        //{
                        //    case State.Off:
                        //        ShinyBleManager.Instance.SetStateChagned(false);
                        //        break;
                        //    case State.On:
                        //        ShinyBleManager.Instance.SetStateChagned(true);
                        //        break;
                        //        //case BluetoothAdapter.STATE_TURNING_OFF: break;
                        //        //case BluetoothAdapter.STATE_TURNING_ON: break;
                        //}
                    }

                    break;
            }
        }
    }
}