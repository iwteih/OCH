﻿using IOCH;
using log4net;
using OCHUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace OCHLib
{
    public enum OCStatus
    {
        Unknown,
        Running,
        NotRunning
    }

    class OCDaemon
    {
        private bool isRuning = true;
        private int sleepTime = 500;
        private OCStatus ocState = OCStatus.Unknown;
        private System.Timers.Timer timer = null;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public delegate void ProcessOfflineHeadle(object sender, OCStatus state);
        public delegate void ProcessOnlineHeadle(object sender, OCStatus state);

        public event ProcessOfflineHeadle OnCommunicatorNotRuning;
        public event ProcessOnlineHeadle OnCommunicatorRuning;

        private EnumProcessesWrapper enumProcesses = new EnumProcessesWrapper();

        public bool IsRuning
        {
            get
            {
                return this.isRuning;
            }
            set
            {
                this.isRuning = value;
            }
        }

        public OCStatus ProcState
        {
            get
            {
                return this.ocState;
            }
        }

        private void ProcessMethod()
        {
            try
            {
                //if (Process.GetProcessesByName("communicator").Length > 0)//RAM goes up and down when using Process.GetProcessesByName
                
                //var processList = enumProcesses.ShowAllProcessName();
                //if (processList.Contains("communicator.exe"))

                if (OCAutomation.CheckCommunicatorUpAndRunning())
                {
                    if ((this.ocState == OCStatus.Unknown) || (this.ocState == OCStatus.NotRunning))
                    {
                        this.ocState = OCStatus.Running;
                        this.OnCommunicatorRuning(this, OCStatus.Running);
                    }
                }
                else
                {
                    ocState = OCStatus.NotRunning;

                    if (OnCommunicatorNotRuning != null)
                    {
                        this.OnCommunicatorNotRuning(this, OCStatus.NotRunning);
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }

            Thread.Sleep(sleepTime);
        }

        public void Start()
        {
            isRuning = true;
            timer = new System.Timers.Timer(sleepTime);
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        public void Stop()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ProcessMethod();
        }
    }
}
