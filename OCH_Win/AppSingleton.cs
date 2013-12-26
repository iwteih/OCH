using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace OCH_Win
{
    class AppSingleton : WindowsFormsApplicationBase
    {
        FrmMain mainForm;

        private const int WS_SHOWNORMAL = 1;
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public AppSingleton()
        {
            this.IsSingleInstance = true;
        }

        protected override bool OnStartup(Microsoft.VisualBasic.ApplicationServices.StartupEventArgs e)
        {
            // First time app is launched
            mainForm = new FrmMain();
            Application.Run(mainForm);
            return false;
        }

        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            // Subsequent launches
            base.OnStartupNextInstance(eventArgs);
            HandleRunningInstance();
        }

        private bool HandleRunningInstance()
        {
            //Ensure to show windows
            ShowWindowAsync(mainForm.Handle, WS_SHOWNORMAL);
            //set windows to foreground
            return SetForegroundWindow(mainForm.Handle);
        }
    }
}
