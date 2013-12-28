namespace OCH_Win
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripView = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAutoStart = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripExit = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBoxContract = new System.Windows.Forms.ListBox();
            this.txtSearchContract = new System.Windows.Forms.TextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listBoxDate = new System.Windows.Forms.ListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnFirstPage = new System.Windows.Forms.ToolStripButton();
            this.btnPrevPage = new System.Windows.Forms.ToolStripButton();
            this.btnNextPage = new System.Windows.Forms.ToolStripButton();
            this.btnLastPage = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.lbDayCount = new System.Windows.Forms.ToolStripLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnReturnDailyView = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearchContent = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerTo = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePickerFrom = new System.Windows.Forms.DateTimePicker();
            this.pnWebbrowser = new System.Windows.Forms.Panel();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.contextMenu.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnWebbrowser.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Visible = true;
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripView,
            this.btnAutoStart,
            this.toolStripExit});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(128, 70);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
            // 
            // toolStripView
            // 
            this.toolStripView.Name = "toolStripView";
            this.toolStripView.Size = new System.Drawing.Size(127, 22);
            this.toolStripView.Text = "History";
            this.toolStripView.Click += new System.EventHandler(this.toolStripView_Click);
            // 
            // btnAutoStart
            // 
            this.btnAutoStart.Checked = true;
            this.btnAutoStart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnAutoStart.Name = "btnAutoStart";
            this.btnAutoStart.Size = new System.Drawing.Size(127, 22);
            this.btnAutoStart.Text = "Auto Start";
            this.btnAutoStart.Click += new System.EventHandler(this.btnAutoStart_Click);
            // 
            // toolStripExit
            // 
            this.toolStripExit.Name = "toolStripExit";
            this.toolStripExit.Size = new System.Drawing.Size(127, 22);
            this.toolStripExit.Text = "Exit";
            this.toolStripExit.Click += new System.EventHandler(this.toolStripExit_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listBoxContract);
            this.splitContainer1.Panel1.Controls.Add(this.txtSearchContract);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(843, 500);
            this.splitContainer1.SplitterDistance = 176;
            this.splitContainer1.TabIndex = 0;
            // 
            // listBoxContract
            // 
            this.listBoxContract.DisplayMember = "ContactName";
            this.listBoxContract.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxContract.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBoxContract.FormattingEnabled = true;
            this.listBoxContract.ItemHeight = 20;
            this.listBoxContract.Location = new System.Drawing.Point(0, 20);
            this.listBoxContract.Name = "listBoxContract";
            this.listBoxContract.Size = new System.Drawing.Size(176, 480);
            this.listBoxContract.TabIndex = 2;
            this.listBoxContract.SelectedIndexChanged += new System.EventHandler(this.listBoxContract_SelectedIndexChanged);
            this.listBoxContract.MouseEnter += new System.EventHandler(this.listBoxContract_MouseEnter);
            // 
            // txtSearchContract
            // 
            this.txtSearchContract.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtSearchContract.Location = new System.Drawing.Point(0, 0);
            this.txtSearchContract.Name = "txtSearchContract";
            this.txtSearchContract.Size = new System.Drawing.Size(176, 20);
            this.txtSearchContract.TabIndex = 0;
            this.txtSearchContract.TextChanged += new System.EventHandler(this.txtSearchContract_TextChanged);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 44);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listBoxDate);
            this.splitContainer2.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pnWebbrowser);
            this.splitContainer2.Size = new System.Drawing.Size(663, 456);
            this.splitContainer2.SplitterDistance = 221;
            this.splitContainer2.TabIndex = 1;
            // 
            // listBoxDate
            // 
            this.listBoxDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBoxDate.FormattingEnabled = true;
            this.listBoxDate.ItemHeight = 20;
            this.listBoxDate.Location = new System.Drawing.Point(0, 0);
            this.listBoxDate.Name = "listBoxDate";
            this.listBoxDate.Size = new System.Drawing.Size(221, 427);
            this.listBoxDate.TabIndex = 2;
            this.listBoxDate.SelectedIndexChanged += new System.EventHandler(this.listBoxDate_SelectedIndexChanged);
            this.listBoxDate.MouseEnter += new System.EventHandler(this.listBoxDate_MouseEnter);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.toolStrip);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 427);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(221, 29);
            this.panel2.TabIndex = 0;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFirstPage,
            this.btnPrevPage,
            this.btnNextPage,
            this.btnLastPage,
            this.toolStripSeparator1,
            this.lbDayCount});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(221, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // btnFirstPage
            // 
            this.btnFirstPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFirstPage.Image = ((System.Drawing.Image)(resources.GetObject("btnFirstPage.Image")));
            this.btnFirstPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFirstPage.Name = "btnFirstPage";
            this.btnFirstPage.Size = new System.Drawing.Size(23, 22);
            this.btnFirstPage.Text = "|<";
            this.btnFirstPage.Click += new System.EventHandler(this.btnFirstPage_Click);
            // 
            // btnPrevPage
            // 
            this.btnPrevPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPrevPage.Image = ((System.Drawing.Image)(resources.GetObject("btnPrevPage.Image")));
            this.btnPrevPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPrevPage.Name = "btnPrevPage";
            this.btnPrevPage.Size = new System.Drawing.Size(27, 22);
            this.btnPrevPage.Text = "<<";
            this.btnPrevPage.Click += new System.EventHandler(this.btnPrevPage_Click);
            // 
            // btnNextPage
            // 
            this.btnNextPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnNextPage.Image = ((System.Drawing.Image)(resources.GetObject("btnNextPage.Image")));
            this.btnNextPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(27, 22);
            this.btnNextPage.Text = ">>";
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            // 
            // btnLastPage
            // 
            this.btnLastPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnLastPage.Image = ((System.Drawing.Image)(resources.GetObject("btnLastPage.Image")));
            this.btnLastPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLastPage.Name = "btnLastPage";
            this.btnLastPage.Size = new System.Drawing.Size(23, 22);
            this.btnLastPage.Text = ">|";
            this.btnLastPage.Click += new System.EventHandler(this.btnLastPage_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // lbDayCount
            // 
            this.lbDayCount.Name = "lbDayCount";
            this.lbDayCount.Size = new System.Drawing.Size(0, 22);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnReturnDailyView);
            this.panel1.Controls.Add(this.btnSearch);
            this.panel1.Controls.Add(this.txtSearchContent);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.dateTimePickerTo);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.dateTimePickerFrom);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(663, 44);
            this.panel1.TabIndex = 0;
            // 
            // btnReturnDailyView
            // 
            this.btnReturnDailyView.Location = new System.Drawing.Point(568, 12);
            this.btnReturnDailyView.Name = "btnReturnDailyView";
            this.btnReturnDailyView.Size = new System.Drawing.Size(58, 23);
            this.btnReturnDailyView.TabIndex = 5;
            this.btnReturnDailyView.Text = "Go Back";
            this.btnReturnDailyView.UseVisualStyleBackColor = true;
            this.btnReturnDailyView.Click += new System.EventHandler(this.btnReturnDailyView_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(504, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(58, 23);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearchContent
            // 
            this.txtSearchContent.Location = new System.Drawing.Point(339, 14);
            this.txtSearchContent.Name = "txtSearchContent";
            this.txtSearchContent.Size = new System.Drawing.Size(159, 20);
            this.txtSearchContent.TabIndex = 4;
            this.txtSearchContent.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchContent_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(170, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "To";
            // 
            // dateTimePickerTo
            // 
            this.dateTimePickerTo.Checked = false;
            this.dateTimePickerTo.Location = new System.Drawing.Point(190, 14);
            this.dateTimePickerTo.Name = "dateTimePickerTo";
            this.dateTimePickerTo.ShowCheckBox = true;
            this.dateTimePickerTo.Size = new System.Drawing.Size(131, 20);
            this.dateTimePickerTo.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "From";
            // 
            // dateTimePickerFrom
            // 
            this.dateTimePickerFrom.Checked = false;
            this.dateTimePickerFrom.Location = new System.Drawing.Point(40, 14);
            this.dateTimePickerFrom.Name = "dateTimePickerFrom";
            this.dateTimePickerFrom.ShowCheckBox = true;
            this.dateTimePickerFrom.Size = new System.Drawing.Size(128, 20);
            this.dateTimePickerFrom.TabIndex = 1;
            // 
            // pnWebbrowser
            // 
            this.pnWebbrowser.Controls.Add(this.webBrowser);
            this.pnWebbrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnWebbrowser.Location = new System.Drawing.Point(0, 0);
            this.pnWebbrowser.Name = "pnWebbrowser";
            this.pnWebbrowser.Size = new System.Drawing.Size(438, 456);
            this.pnWebbrowser.TabIndex = 0;
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(438, 456);
            this.webBrowser.TabIndex = 4;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 500);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OCH";
            this.Activated += new System.EventHandler(this.FrmMain_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.contextMenu.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnWebbrowser.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripView;
        private System.Windows.Forms.ToolStripMenuItem toolStripExit;
        private System.Windows.Forms.TextBox txtSearchContract;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnFirstPage;
        private System.Windows.Forms.ToolStripButton btnPrevPage;
        private System.Windows.Forms.ToolStripButton btnNextPage;
        private System.Windows.Forms.ToolStripButton btnLastPage;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSearchContent;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dateTimePickerTo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePickerFrom;
        private System.Windows.Forms.ToolStripMenuItem btnAutoStart;
        private System.Windows.Forms.ToolStripLabel lbDayCount;
        private System.Windows.Forms.ListBox listBoxContract;
        private System.Windows.Forms.Button btnReturnDailyView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ListBox listBoxDate;
        private System.Windows.Forms.Panel pnWebbrowser;
        private System.Windows.Forms.WebBrowser webBrowser;
    }
}

