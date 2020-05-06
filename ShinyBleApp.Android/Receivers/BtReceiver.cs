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
                        App.AddLog($"{MethodBase.GetCurrentMethod().Name} {Helper.GetEnumName(outState)}");
                        switch (outState)
                        {
                            case State.Off:
                                //
                                // if i dont call this function, device status don't be changed on LG V30
                                ShinyBleApp.Models.Managers.BleManager.Instance.SetBleStatus(false);
                                break;
                            case State.On:
                                ShinyBleApp.Models.Managers.BleManager.Instance.SetBleStatus(true);
                                break;
                                //case BluetoothAdapter.STATE_TURNING_OFF: break;
                                //case BluetoothAdapter.STATE_TURNING_ON: break;
                        }
                    }

                    break;
            }
        }
    }
}