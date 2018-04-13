using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

namespace AutomationHelper
{
    public class SystemUtility
    {
        #region Windows methods
        public static string GetWindowsLogonName()
        {
            string[] winUserName = WindowsIdentity.GetCurrent().Name.Split('\\');
            return (winUserName.Length >= 2) ? winUserName[1] : (winUserName.Length <= 0) ? string.Empty : winUserName[0];
        }

        public static string GetComputerName()
        {
            return System.Environment.MachineName;
        }
        #endregion
    }
}
