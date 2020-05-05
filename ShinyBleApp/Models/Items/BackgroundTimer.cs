using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Timers;

namespace ShinyBleApp.Models.Items
{
    public class BackgroundTimer
    {
        public bool IsStarted { get; private set; }

        Timer _timer;
        BackgroundWorker _backgroundWorker;

        public event DoWorkEventHandler DoWork
        {
            add
            {
                _backgroundWorker.DoWork += value;
            }
            remove
            {
                _backgroundWorker.DoWork -= value;
            }
        }

        public event RunWorkerCompletedEventHandler RunWorkerCompleted
        {
            add
            {
                _backgroundWorker.RunWorkerCompleted += value;
            }
            remove
            {
                _backgroundWorker.RunWorkerCompleted -= value;
            }
        }

        public event ElapsedEventHandler Elapsed
        {
            add
            {
                _timer.Elapsed += value;
            }
            remove
            {
                _timer.Elapsed -= value;
            }
        }
        public double Interval { set { _timer.Interval = value; } }

        public bool CancellationPending { get => _backgroundWorker.CancellationPending; }


        public BackgroundTimer()
        {
            IsStarted = false;
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.WorkerSupportsCancellation = true;
            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
        }

        public void Start()
        {
            if (!IsStarted)
            {
                Timer_Elapsed(_timer, default);
                IsStarted = true;
                _timer.Start();
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                IsStarted = false;
                _backgroundWorker.CancelAsync();
                _timer.Stop();
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // e is not sure default
            if (_backgroundWorker.IsBusy)
            {
                return;
            }

            _backgroundWorker.RunWorkerAsync();
        }
    }
}
