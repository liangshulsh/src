using System;
using System.Configuration.Install;
using System.ComponentModel;
using System.ServiceProcess;

namespace InteractiveProcessFromService
{
    [RunInstaller(true)]
    public class InteractiveProcessInstaller : Installer
    {
        //Standard service installation.
        public InteractiveProcessInstaller()
        {
            ServiceProcessInstaller pi = new ServiceProcessInstaller();
            ServiceInstaller si = new ServiceInstaller();

            //Launching into another session like this requires the service
            //account to have the SE_TCB_NAME (Act as part of the operating 
            //system) privilege. By default only the loacl system account 
            //has this privilege.
            pi.Account = ServiceAccount.LocalSystem;
            si.ServiceName = "InteractiveProcess";
            si.Description = "Launches processes into interactive sessions.";
            si.DisplayName = "Interactive Process Launcher";

            this.Installers.AddRange(new Installer[] { pi, si });
        }
    }
}
