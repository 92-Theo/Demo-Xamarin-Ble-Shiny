using ShinyBleApp.Models.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace ShinyBleApp.Models.Managers
{
    public class TimerManager
    {
        static Lazy<TimerManager> _lazy = new Lazy<TimerManager>(() => new TimerManager());
        public static TimerManager Instance { get => _lazy.Value; }

        TimerManager()
        {
            ScanTimer = new BackgroundTimer();
            ScanTimer.Interval = Constants.Ble.ScanInterval;
            ScanTimer.DoWork += new DoWorkEventHandler(ScanTimer_DoWork);
            ScanTimer.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ScanTimer_RunWorkerCompleted);
        }

        #region Scan

        public BackgroundTimer ScanTimer { get; private set; }

        private void ScanTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            // Console.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + " backgroundWorker1_DoWork");
            // 취소 버튼 클릭 했을 경우
            if (ScanTimer.CancellationPending)
            {
                e.Cancel = true; //작업취소
                return;
            }

            ScanWorkThread().ContinueWith(task =>
            {
                Console.WriteLine(task.Result);
                e.Result = task.Result;
            }).Wait();
            Console.WriteLine("backgroundWorker1_DoWork End");
        }

        public async Task<string> ScanWorkThread()
        {
            if (StatusManager.Instance.IsBleOn)
            {
                await BleManager.Instance.StopScan();
                await BleManager.Instance.StartScan(Constants.Ble.ScanTimeout);
            }
            else
            {
                App.AddLog("ScanWorkThread Invalidate");
            }

            return string.Empty;
        }


        private void ScanTimer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //App.AddLog(System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() + " backgroundWorker1_RunWorkerCompleted");
            if (e.Cancelled)
            {
                //ResultMsg = "작업 취소됨";
            }
            else if (e.Error != null)
            {
                // 에러 발생시 메시지 표시
                //ResultMsg = e.Error.Message.ToString();
            }
            else
            {
                if (e.Result != null)
                {
                    //ResultMsg = e.Result.ToString();
                }
            }
        }
        #endregion
    }
}
