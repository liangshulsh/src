using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace LaunchProcessFromService
{
    [RunInstaller(true)]
    public class LauncherInstaller : Installer
    {
        //Standard service installation.
        public LauncherInstaller()
        {
            ServiceProcessInstaller pi = new ServiceProcessInstaller();
            ServiceInstaller si = new ServiceInstaller();

            //If the service is running under LocalSystem it cannot launch processes
            //under new credentials, any attempt to do so will throw a Win32Exception
            //(ERROR_ACCESS_DENIED).
            pi.Account = ServiceAccount.LocalService;
            si.ServiceName = "ProcessLauncher";
            si.Description = "Launches processes under new credentials.";
            si.DisplayName = "Process Launcher";

            this.Installers.AddRange(new Installer[] { pi, si });
        }
    }
}
