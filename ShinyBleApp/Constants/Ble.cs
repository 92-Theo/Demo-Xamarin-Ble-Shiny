using Shiny.BluetoothLE.Central;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShinyBleApp.Constants
{
    public static class Ble
    {
        public static readonly ScanConfig ScanConfig = new ScanConfig() { ServiceUuids = new List<Guid>() { ServiceGuid } };
        public static readonly ConnectionConfig AutoConnectionConfig = new ConnectionConfig() { AutoConnect = true };
        public static readonly ConnectionConfig ConnectionConfig = new ConnectionConfig() { AutoConnect = false };

        public static readonly Guid ServiceGuid = Guid.NewGuid();
        public static readonly Guid ReadGattCharGuid = Guid.NewGuid();
        public static readonly Guid WriteGattCharGuid = Guid.NewGuid();


        public static readonly TimeSpan OperationTimeout = TimeSpan.FromSeconds(1);
        public const double ScanInterval = 7000;
        public static readonly TimeSpan ScanTimeout = TimeSpan.FromMilliseconds(400);
        public static readonly TimeSpan GetKnownPeripheralTimeout = TimeSpan.FromSeconds(1);
    }
}
