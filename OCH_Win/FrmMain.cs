using IOCH;
using log4net;
using Microsoft.Win32;
using mshtml;
using OCHEntity;
using OCHLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace OCH_Win
{
    public partial class FrmMain : Form, INotify
    {
        private IMessageStore messageStore = null;
        private MessageProvider messageProvider = null;
        private Contact selectedContract = null;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string APPLICATIONNAME = "OCH";
        private const string REGISTRYPATH = @"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\";

        private FrmLoading loading = new FrmLoading();
        private BackgroundWorker loadWorker = new BackgroundWorker();

        public FrmMain()
        {
            InitializeComponent();

            log4net.Config.XmlConfigurator.Configure();

            Initialize();
            SetVisibility();
            StartRecord();
        }

        private void Initialize()
        {
            messageStore = new MessageStoreImpl.SQLite();
            //messageStore = new MessageStoreImpl.SQL();
            //messageStore = new MessageStoreImpl.STSdb(Environment.UserName);
            messageProvider = new DailyMessageProvider(messageStore);

            webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;

            loadWorker.DoWork += loadWorker_DoWork;
            loadWorker.RunWorkerCompleted += loadWorker_RunWorkerCompleted;
        }

        void loadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (loading.Visible)
            {
                loading.Close();
            }

            AddToListView(messageProvider.ContractList);
        }

        void loadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            messageProvider.LoadContracts();
        }
        
        void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HighlightKeyword();
        }

        private void StartRecord()
        {
            string user = Environment.UserName;

            OCHMessage och = new OCHMessage(messageStore, this);
            och.StartRecord();
        }

        private void ShowMainForm()
        {
            this.WindowState = FormWindowState.Normal;
            this.Show();
            this.Activate();
        }

        private void SetVisibility()
        {
            notifyIcon.MouseClick += delegate(object sender, MouseEventArgs args)
            {
                if (args.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    ShowMainForm();
                }
            };
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.Visible = false;
            this.ShowInTaskbar = false;
            notifyIcon.Visible = true;
            LoadContracts();

            //DateTime dt = DateTime.Now;
            //for (int i = 0; i < 400; i++)
            //{
            //    messageStore.SaveMessage(DateTime.Now.AddDays(i), string.Format("Fake message -- {0} @ {1}", i, DateTime.Now.AddDays(i)), new string[] { "fake@fake.com" });
            //}
            //Console.WriteLine(DateTime.Now - dt);

            //Console.WriteLine("Done");
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
        }

        private void toolStripView_Click(object sender, EventArgs e)
        {
            ShowMainForm();
        }

        private void toolStripExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void AddToListView(IEnumerable<Contact> list)
        {
            listBoxContract.Items.Clear();

            if (list != null && list.Count() > 0)
            {
                foreach (var item in list)
                {
                    listBoxContract.Items.Add(item);
                }
            }

            //if (listBoxContract.Items.Count == 1)
            //{
            //    listBoxContract.SetSelected(0, true);
            //}
        }

        private void LoadContracts()
        {
            if (messageProvider != null && !loadWorker.IsBusy)
            {
                loadWorker.RunWorkerAsync();
            }
        }

        private void txtSearchContract_TextChanged(object sender, EventArgs e)
        {
            if (messageProvider != null)
            {
                var filterList = messageProvider.ContractList.Where(
                    f => f.ContactName.ToLower().
                        IndexOf(txtSearchContract.Text.ToLower()) != -1);
                AddToListView(filterList);
            }
        }

        private void ShowDateList(DateTime? baseDate, SearchDirection direction = SearchDirection.None)
        {
            Contact contract = selectedContract;

            if (contract != null)
            {
                var list = messageProvider.GetConversationDateList(contract, baseDate, direction);

                if (list != null && list.Count > 0)
                {
                    listBoxDate.Items.Clear();

                    foreach (var l in list)
                    {
                        listBoxDate.Items.Add(l.ToString("yyyy-MM-dd"));
                    }

                    ForceWebBrowserShowContent(string.Empty);
                }

                if (list != null && list.Count > 0)
                {
                    listBoxDate.SetSelected(listBoxDate.Items.Count - 1, true);
                }
            }
        }

        private void ShowTotalCount(Contact contract)
        {
            int total = messageProvider.GetTotalDailyMessageCount(contract);

            lbDayCount.Text = total.ToString();
        }

        private void listBoxContract_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxContract.SelectedItems != null
                && listBoxContract.SelectedItems.Count > 0)
            {
                Contact contract = listBoxContract.SelectedItems[0] as Contact;

                selectedContract = contract;
            }

            if (selectedContract != null)
            {
                ShowTotalCount(selectedContract);
                ShowDateList(null, SearchDirection.Last);
            }
        }

        private void listBoxContract_MouseEnter(object sender, EventArgs e)
        {
            listBoxContract.Focus();
        }

        private void FrmMain_Activated(object sender, EventArgs e)
        {
            LoadContracts();
        }

        //workaround for webBrowser.DocumentText show <html></html> when first time invoke
        private void ForceWebBrowserShowContent(string html)
        {
            //workaroud 1. 
            //webBrowser.Navigate("about:blank");
            //HtmlDocument doc = this.webBrowser.Document;
            //doc.Write(String.Empty);
            //webBrowser.DocumentText = html;
            
            //workaroud 2.
            webBrowser.DocumentText = html;
            Application.DoEvents();
        }

        private int HighlightKeyword()
        {
            int matches = -1;

            if (!string.IsNullOrEmpty(messageProvider.Keyword))
            {
                const string strbg = "BackColor";
                const string strf = "ForeColor";
                const string sword = "Character";
                const string stextedit = "Textedit";

                //You may be tempted to use Color.Yellow.ToArgb(). But,
                //the value returned includes the A value which
                //causes confusion for MSHTML. i.e. Color.Cyan is interpreted as yellow
                int background = MakeRGB(Color.FromName("Yellow"));
                //int foreground = MakeRGB(Color.FromName("Yellow"));

                IHTMLDocument2 document = webBrowser.Document.DomDocument as IHTMLDocument2;

                if (document != null)
                {
                    IHTMLElement pElem = document.body;
                    IHTMLBodyElement pBodyelem = pElem as IHTMLBodyElement;
                    if (pBodyelem == null)
                        return matches;

                    IHTMLTxtRange range = pBodyelem.createTextRange();
                    if (range == null)
                        return matches;

                    //IHTMLSelectionObject currentSelection = document.selection;
                    //IHTMLTxtRange range = currentSelection.createRange() as IHTMLTxtRange;

                    IHTMLTxtRange firstRange = null;  

                    while (range.findText(messageProvider.Keyword, messageProvider.Keyword.Length, 0))
                    {
                        if (matches == -1)
                        {
                            matches = 0;
                            firstRange = range.duplicate();
                        }

                        if (background != 0)
                            range.execCommand(strbg, false, background);
                        //if (foreground != 0)
                        //range.execCommand(strf, false, foreground);
                        range.moveStart(sword, 1);
                        range.moveEnd(stextedit, 1);
                        matches += 1;
                    }

                    if (firstRange != null)
                    {
                        firstRange.scrollIntoView(true);
                    }
                }
            }

            return matches;
        }

        private int MakeRGB(Color cColor)
        {
            try
            {
                return (int)((cColor.R | (((short)cColor.G) << 8)) | (((int)cColor.B) << 16));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private void ShowMessage()
        {
            Contact contract = null;

            if (selectedContract != null)
            {
                contract = selectedContract;
            }

            if (contract != null
                && listBoxDate.SelectedItems != null
                && listBoxDate.SelectedItems.Count > 0)
            {
                DateTime dt = DateTime.Parse(listBoxDate.SelectedItems[0].ToString());
                string html = messageProvider.GetDailyConversation(contract, dt);

                ForceWebBrowserShowContent(html);
            }
        }

        private void listBoxDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowMessage();
        }

        private void listBoxDate_MouseEnter(object sender, EventArgs e)
        {
            listBoxDate.Focus();
        }

        private string ReplaceBetween(string s, string begin, string end, string replace)
        {
            Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
            return regex.Replace(s, replace);
        }


        private string ReplaceHistory(string history)
        {
            history = ReplaceBetween(history, "<OBJECT", "</OBJECT>", "&#8227");//"&#149;"
            history = history.Replace("<A ", "<A target=\"blank\" ");
            return history;
        }

        private void Search()
        { 
            DateTime dtStart = dateTimePickerFrom.Checked ? dateTimePickerFrom.Value.Date : DateTime.MinValue;
            DateTime dtEnd = dateTimePickerTo.Checked ? dateTimePickerTo.Value.Date.AddDays(1) : DateTime.MaxValue;
            string keyword = txtSearchContent.Text;

            listBoxDate.Items.Clear();
            ForceWebBrowserShowContent(string.Empty);
            lbDayCount.Text = string.Empty;

            selectedContract = null;

            messageProvider = new Search4MessageProvider(messageStore, dtStart, dtEnd, keyword);
            LoadContracts();
            loading.ShowDialog();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void btnReturnDailyView_Click(object sender, EventArgs e)
        {
            listBoxDate.Items.Clear();
            ForceWebBrowserShowContent(string.Empty);
            lbDayCount.Text = string.Empty;
            dateTimePickerFrom.Checked = false;
            dateTimePickerTo.Checked = false;
            txtSearchContent.Clear();

            messageProvider = new DailyMessageProvider(messageStore);
            LoadContracts();
            loading.ShowDialog();
        }

        #region Dummy Notify
        public void Connected()
        {

        }

        public void NotConnect()
        {

        }

        public void OCNotRuning()
        {

        }

        public void OCRunning()
        {

        }
        #endregion

        private DateTime? GetTopDateTimeInListView()
        {
            if (listBoxDate.Items.Count > 0)
            {
                return DateTime.Parse(listBoxDate.Items[0].ToString());
            }

            return null;
        }

        private DateTime? GetBottomDateTimeInListView()
        {
            if (listBoxDate.Items.Count > 0)
            {
                return DateTime.Parse(listBoxDate.Items[listBoxDate.Items.Count - 1].ToString());
            }

            return null;
        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            DateTime? baseDate = GetTopDateTimeInListView();

            if (baseDate != null)
            {
                ShowDateList(baseDate.Value, SearchDirection.Backward);
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            DateTime? baseDate = GetBottomDateTimeInListView();

            if (baseDate != null)
            {
                ShowDateList(baseDate.Value, SearchDirection.Forward);
            }
        }

        private void btnFirstPage_Click(object sender, EventArgs e)
        {
            ShowDateList(DateTime.MinValue, SearchDirection.First);
        }

        private void btnLastPage_Click(object sender, EventArgs e)
        {
            ShowDateList(DateTime.MaxValue, SearchDirection.Last);
        }

        private void btnAutoStart_Click(object sender, EventArgs e)
        {
            btnAutoStart.Checked = !btnAutoStart.Checked;

            string appStartPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            bool canSetRegex = RunWhenStart(btnAutoStart.Checked, APPLICATIONNAME, System.IO.Path.Combine(appStartPath, APPLICATIONNAME + ".exe"));

            if (!canSetRegex)
            {
                MessageBox.Show(string.Format("Oh my!{0}Please try administrator role...", Environment.NewLine), APPLICATIONNAME);
            }
        }

        #region Regedit AutoRun
        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            btnAutoStart.Checked = IsRegeditExisted(APPLICATIONNAME);
        }

        private bool RunWhenStart(bool started, string name, string path)
        {
            bool success = false;

            RegistryKey HKLM = null;
            RegistryKey Run = null;

            try
            {
                HKLM = Registry.LocalMachine;
                Run = HKLM.CreateSubKey(REGISTRYPATH);

                if (started)
                {
                    Run.SetValue(name, path);
                }
                else
                {
                    if (IsRegeditExisted(name))
                    {
                        Run.DeleteValue(name);
                    }
                }

                success = true;
            }
            catch (Exception exp)
            {
                logger.Error(exp);
            }
            finally
            {
                if (Run != null)
                {
                    Run.Close();
                }

                if (HKLM != null)
                {
                    HKLM.Close();
                }
            }

            return success;
        }

        private bool IsRegeditExisted(string name)
        {
            bool existed = false;

            RegistryKey hkml = Registry.LocalMachine;
            RegistryKey software = null;

            try
            {
                string[] subkeyNames;

                software = hkml.OpenSubKey(REGISTRYPATH, true);
                subkeyNames = software.GetValueNames();
                foreach (string keyName in subkeyNames)
                {
                    if (keyName == name)
                    {
                        existed = true;
                    }
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp);
            }
            finally
            {
                if (hkml != null)
                {
                    hkml.Close();
                }

                if (software != null)
                {
                    software.Close();
                }
            }

            return existed;
        }
        #endregion

        private void txtSearchContent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Search();
            }
        }



    }
}
