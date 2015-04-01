using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;

using System.Threading;
using System.Runtime.InteropServices;

namespace InteractiveProcessFromService
{
    partial class InteractiveProcess : ServiceBase
    {
        public InteractiveProcess()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ThreadPool.QueueUserWorkItem(LaunchService);
        }

        public void LaunchService(object context)
        {
            IntPtr hSessionToken = IntPtr.Zero;
            try
            {
                SessionFinder sf = new SessionFinder();
                //Get the ineractive console session.
                hSessionToken = sf.GetLocalInteractiveSession();
                
                //Use this instead to get the session of a specific user.
                //hSessionToken = sf.GetSessionByUser(Environment.MachineName, "InteractiveLaunchUser");
                sf.GetSessionByUser(
                if (hSessionToken != IntPtr.Zero)
                {
                    //Run notepad in the session that we found using the default
                    //values for working directory and desktop.
                    InteractiveProcessRunner runner = 
                        new InteractiveProcessRunner("notepad.exe", hSessionToken);
                    runner.Run();
                }
                else
                {
                    EventLog.WriteEntry("Session not found.", EventLogEntryType.Error);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(string.Format("Exception thrown: {0}{0}{1}", Environment.NewLine, ex), EventLogEntryType.Error);
            }
            finally
            {
                if (hSessionToken != IntPtr.Zero)
                {
                    CloseHandle(hSessionToken);
                }
            }
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObj);

    }
}
