using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShinyBleApp.Models.Managers
{
    public class ViewManager
    {
        public static void InvokeOnMainThread(Action action)
        {
            Device.BeginInvokeOnMainThread(action);
        }

        public static async Task InvokeOnMainThreadAsync(Action action)
        {
            await Device.InvokeOnMainThreadAsync(action);
        }
    }
}
