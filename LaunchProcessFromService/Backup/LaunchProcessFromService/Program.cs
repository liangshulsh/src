using System;
using System.ServiceProcess;

namespace LaunchProcessFromService
{
    static class Program
    {


        static void Main(string[] args)
        {
            ProcessLauncher service = new ProcessLauncher();
            if (args.Length > 0 && args[0] == "/i")
            {
                //Launched not as service.
                service.LaunchCommandLine();
            }
            else
            {
                //Standard service entry point.
                ServiceBase.Run(service);
            }
        }
    }
}
