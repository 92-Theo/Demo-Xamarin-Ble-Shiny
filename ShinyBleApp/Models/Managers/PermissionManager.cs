using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShinyBleApp.Models.Managers
{
    public class PermissionManager
    {
        static Lazy<PermissionManager> _lazy = new Lazy<PermissionManager>(() => new PermissionManager());
        public static PermissionManager Instance { get => _lazy.Value; }


        public bool isLocation { get; set; } = false;


        public async Task<bool> RequestLocation()
        {
            if (isLocation)
                return true;

            var status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                {
                    //await App.Current.MainPage.DisplayAlert("Need location", "Gunna need that location", "OK");
                }

                var result = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
                bool bRet = result == PermissionStatus.Granted;
                isLocation = bRet;
                return bRet;
            }
            else
            {
                isLocation = true;
                return true;
            }
        }
    }
}
