using CommunicatorAPI;
using IOCH;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace OCHLib
{
    public class OCHMessage
    {
        private OCAutomation automation;

        private List<OCMessageWindow> messageWindowList = new List<OCMessageWindow>();

        private bool isEventBinded = false;
        private bool isConnected = false;
        private OCDaemon daemon;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IMessageStore messageStore;
        private INotify notify;

        private Regex EMPTYMESSAGE = new Regex(@"^<DIV>\s*<DIV id=finalPadding style=""PADDING-BOTTOM: 3px; PADDING-TOP: 3px; PADDING-LEFT: 3px; PADDING-RIGHT: 3px"">\s*</DIV>\s*</DIV>$");

        public OCHMessage() { }

        public OCHMessage(IMessageStore messageStore, INotify notify)
        {
            if (messageStore == null)
            {
                throw new ArgumentNullException("IMessageStore is null");
            }

            if (notify == null)
            {
                throw new ArgumentNullException("INotify is null");
            }

            this.messageStore = messageStore;
            this.notify = notify;
        }

        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }
        }

        private void BindOCAutomationEvent()
        {
            if (isEventBinded)
                return;

            try
            {

                this.automation = OCAutomation.GetInstance();
                //automation.ConnectionStateChanged -= new EventHandler<ConnectionStateChangedEventArgs>(ConnectionStateChanged);
                automation.ConnectionStateChanged += new EventHandler<ConnectionStateChangedEventArgs>(ConnectionStateChanged);
                //automation.IMWindowContactAdded -= new EventHandler<IMWindowContactAddedEventArgs>(IMWindowContactAdded);
                //automation.IMWindowCreated -= new EventHandler<IMWindowCreatedEventArgs>(IMWindowCreated);
                //automation.IMWindowDestroyed -= new EventHandler<IMWindowDestroyedEventArgs>(IMWindowDestroyed);
                //automation.Signin -= new EventHandler<SigninEventArgs>(Signin);
                //automation.Signout -= new EventHandler<EventArgs>(Signout);
                automation.IMWindowContactAdded += new EventHandler<IMWindowContactAddedEventArgs>(IMWindowContactAdded);
                automation.IMWindowCreated += new EventHandler<IMWindowCreatedEventArgs>(IMWindowCreated);
                automation.IMWindowDestroyed += new EventHandler<IMWindowDestroyedEventArgs>(IMWindowDestroyed);
                automation.Signin += new EventHandler<SigninEventArgs>(Signin);
                automation.Signout += new EventHandler<EventArgs>(Signout);

                isEventBinded = true;
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }
        }

        private void ConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            this.isConnected = e.Connected;
        }
        
        private OCMessageWindow GetMessageWindow(string strWinID)
        {
            try
            {
                return messageWindowList.Where(f => f.WindowHWND == strWinID).FirstOrDefault();
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }
            return null;
        }

        private void IMWindowContactAdded(object sender, IMWindowContactAddedEventArgs e)
        {
            try
            {
                string winID = e.pIMWindow.GetHashCode().ToString();
                IMessengerWindow pIMWindow = (IMessengerWindow)e.pIMWindow;
                IMessengerConversationWnd wnd = (IMessengerConversationWnd)pIMWindow;
                IMessengerContacts contacts = (IMessengerContacts)wnd.Contacts;

                if (contacts.Count > 1)
                {
                    List<string> list = new List<string>(contacts.Count);
                    
                    for (int i = 0; i < contacts.Count; i++)
                    {
                        list.Add(((IMessengerContact)contacts.Item(i)).SigninName);
                    }

                    OCMessageWindow windows = GetMessageWindow(winID);
                    windows.ContactsList.AddRange(list.Where(f => !windows.ContactsList.Contains(f)));
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }
        }

        private void IMWindowCreated(object sender, IMWindowCreatedEventArgs e)
        {
            try
            {
                OCMessageWindow windows = new OCMessageWindow();
                IMessengerWindow pIMWindow = (IMessengerWindow)e.pIMWindow;
                IMessengerConversationWnd wnd = (IMessengerConversationWnd)pIMWindow;
                windows.WindowHWND = e.pIMWindow.GetHashCode().ToString();
                windows.BeginTime = DateTime.Now;

                IMessengerContacts contacts = (IMessengerContacts)wnd.Contacts;
                
                for (int i = 0; i < contacts.Count; i++)
                {
                    windows.ContactsList.Add(((IMessengerContact)contacts.Item(i)).SigninName);
                }
                
                messageWindowList.Add(windows);
                windows.Run(pIMWindow);
                windows.SaveHistroyEvent += windows_SaveHistroyEvent;
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }
        }

        private void windows_SaveHistroyEvent(object sender, string message)
        {
            OCMessageWindow window = sender as OCMessageWindow;

            if (window != null)
            {
                if (EMPTYMESSAGE.IsMatch(message))
                {
                    return;
                }

                string[] array = new string[window.ContactsList.Count];
                window.ContactsList.CopyTo(array);
                messageStore.SaveMessage(window.BeginTime, message, array);
            }
        }

        private void IMWindowDestroyed(object sender, IMWindowDestroyedEventArgs e)
        {
            try
            {
                OCMessageWindow window = GetMessageWindow(e.pIMWindow.GetHashCode().ToString());
                if (window != null)
                {
                    window.Stop();
                    this.messageWindowList.Remove(window);
                }
                else
                {
                    logger.Warn("Cannot retrieve IMWindow");
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }
        }

        public void StartRecord()
        {
            try
            {
                daemon = new OCDaemon();
                daemon.OnCommunicatorRuning += daemon_OnCommunicatorRuning;
                daemon.OnCommunicatorNotRuning += daemon_OnCommunicatorNotRuning;

                daemon.Run();

                //BindOCAutomationEvent();
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }
        }

        private void daemon_OnCommunicatorNotRuning(object sender, OCStatus state)
        {
            try
            {
                this.isConnected = false;
                this.automation.ShutdownApp();

                notify.OCNotRuning();
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }
        }

        private void daemon_OnCommunicatorRuning(object sender, OCStatus state)
        {
            try
            {
                BindOCAutomationEvent();
                string textStatus = automation.GetTextStatus(automation.GetSignedInUser());

                if ((this.isConnected 
                    && (textStatus != "Unknown"))
                    && (textStatus != "Offline"))
                {
                    notify.Connected();
                }
                else
                {
                    notify.NotConnect();                 
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }
        }

        private void Signin(object sender, SigninEventArgs e)
        {
            try
            {
                notify.Connected();
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }
        }

        private void Signout(object sender, EventArgs e)
        {
            try
            {
                isEventBinded = false;
                notify.NotConnect();
            }
            catch (Exception exception)
            {
                logger.Error(exception);
            }
        }        
    }
}
