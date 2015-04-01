using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

using Asprosys.Security.AccessControl;

namespace LaunchProcessFromService
{
    partial class ProcessLauncher : ServiceBase
    {
        public ProcessLauncher()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //Start service work.
            ThreadPool.QueueUserWorkItem(LaunchService);
        }

        public void LaunchCommandLine()
        {
            EventLog.WriteEntry("Running with command line /i.", EventLogEntryType.Information);
        }

        public void LaunchService(object context)
        {

            //The user that the process will be started under must have Read and 
            //Execute access to this file, not the service account! If the user does 
            //not have this access Process.Start will throw a Win32Exception
            //(ERROR_ACCESS_DENIED).
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            ProcessStartInfo psi = new ProcessStartInfo(path);
            psi.Arguments = "/i";
            psi.CreateNoWindow = true;

            //Must be set if running under new credentials.
            psi.UseShellExecute = false;

            //This must be set to a valid directory. If left to the default 
            //you will get a Win32Exception - invalid directory error.
            psi.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System);

            //Set user
            psi.Domain = Environment.MachineName;
            psi.UserName = "LaunchProcessUser";
            psi.Password = new System.Security.SecureString();
            psi.Password.AppendChar('t');
            psi.Password.AppendChar('e');
            psi.Password.AppendChar('s');
            psi.Password.AppendChar('t');

            try
            {

                //The following security adjustments are necessary to give the new 
                //process sufficient permission to run in the service's window station
                //and desktop. This uses classes from the AsproLock library also from 
                //Asprosys.
                IntPtr hWinSta = GetProcessWindowStation();
                WindowStationSecurity ws = new WindowStationSecurity(hWinSta,
                  System.Security.AccessControl.AccessControlSections.Access);
                ws.AddAccessRule(new WindowStationAccessRule("LaunchProcessUser",
                    WindowStationRights.AllAccess, System.Security.AccessControl.AccessControlType.Allow));
                ws.AcceptChanges();

                IntPtr hDesk = GetThreadDesktop(GetCurrentThreadId());
                DesktopSecurity ds = new DesktopSecurity(hDesk,
                    System.Security.AccessControl.AccessControlSections.Access);
                ds.AddAccessRule(new DesktopAccessRule("LaunchProcessUser",
                    DesktopRights.AllAccess, System.Security.AccessControl.AccessControlType.Allow));
                ds.AcceptChanges();

                EventLog.WriteEntry("Launching application.", EventLogEntryType.Information);

                using (Process process = Process.Start(psi))
                {
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(string.Format("Exception thrown:{0}{0}{1}", Environment.NewLine, ex), EventLogEntryType.Error);
            }

        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetProcessWindowStation();


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetThreadDesktop(int dwThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetCurrentThreadId();
    }
}
