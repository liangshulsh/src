using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace Skywolf
{
    public partial class SkywolfRibbon
    {
        public delegate Int32 CallBack(ref long a);

        CallBack mycall;

        [DllImport("kernel32")]
        private static extern void RtlMoveMemory(ref byte dst,
        ref byte src, Int32 len);

        [DllImport("kernel32")]
        private static extern Int32 SetUnhandledExceptionFilter(CallBack cb);

        public static Int32 newexceptionfilter(ref long a)
        {
            MessageBox.Show("截获了全局异常!");
            return 0;
        }

        private void SkywolfRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            mycall = new CallBack(newexceptionfilter);
            SetUnhandledExceptionFilter(mycall);
            AppDomain.CurrentDomain.FirstChanceException += new EventHandler<System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs>(CurrentDomain_FirstChanceException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
        }

        void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show("Application exception catched");
        }

        void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            MessageBox.Show("first exception catched");
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("exception catched");
        }

        private void ButtonClick(object sender, RibbonControlEventArgs e)
        {
            try
            {
                Globals.ThisAddIn.Application.Workbooks[100].Activate();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            throw new InvalidOperationException("No");
        }
    }
}
