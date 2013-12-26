using CommunicatorAPI;
using IOCH;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace OCHLib
{
    internal struct ThreadParameter
    {
        public AutoResetEvent doneEvent;
        public IMessengerWindow IMWindow;
    }

    public class OCMessageWindow
    {
        private Thread saveThread;
        private AutoResetEvent doneEvent;
        private DateTime beginTime;
        private bool isRuning = true;
        private long savedMessageWordsCount = 0;
        private List<string> contactsList = new List<string>();
        private string windowHWND = string.Empty;

        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public delegate void SaveHistroyTextHardler(object sender, string strHistoryText);
        public event SaveHistroyTextHardler SaveHistroyEvent;

        public void Run(IMessengerWindow iMWindow)
        {
            try
            {
                doneEvent = new AutoResetEvent(false);
                ThreadParameter parameter = new ThreadParameter();
                parameter.doneEvent = this.doneEvent;
                parameter.IMWindow = iMWindow;
                saveThread = new Thread(new ParameterizedThreadStart(this.SaveHistroyMethod));
                saveThread.Start(parameter);
            }
            catch (COMException comExp)
            {
                logger.Error(comExp);
            }
            catch (Exception exp)
            {
                logger.Error(exp);
            }
        }

        private void SaveHistroyMethod(object obj)
        {
            ThreadParameter para = (ThreadParameter)obj;
            IMessengerConversationWnd wnd = para.IMWindow as IMessengerConversationWnd;
            string history = string.Empty;

            while (isRuning)
            {
                try
                {
                    if (wnd != null
                        && !string.IsNullOrEmpty(wnd.History)
                        && (savedMessageWordsCount != wnd.History.Length))
                    {
                        history = wnd.History;
                        savedMessageWordsCount = history.Length;

                        if (this.SaveHistroyEvent != null)
                        {
                            lock (this.SaveHistroyEvent)
                            {
                                this.SaveHistroyEvent(this, history);
                            }
                        }
                    }
                }
                catch (COMException comExp)
                {
                    //logger.Warn(exp);
                    Console.WriteLine(comExp);
                }
                catch (Exception exp)
                {
                    logger.Error(exp);
                    this.isRuning = false;
                }
                Thread.Sleep(200);
            }

            para.doneEvent.Set();
        }

        public void Stop()
        {
            try
            {
                this.isRuning = false;
                this.doneEvent.WaitOne();
                saveThread.Abort();
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }
        }

        public DateTime BeginTime
        {
            get
            {
                return this.beginTime;
            }
            set
            {
                this.beginTime = value;
            }
        }

        public List<string> ContactsList
        {
            get
            {
                return this.contactsList;
            }
            set
            {
                this.contactsList = value;
            }
        }

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

        public string WindowHWND
        {
            get
            {
                return this.windowHWND;
            }
            set
            {
                this.windowHWND = value;
            }
        }

        public long SessionId
        {
            get
            {
                return BeginTime.Ticks;
            }
        }
    }
}
