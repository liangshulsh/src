using System;
using System.ServiceProcess;

namespace InteractiveProcessFromService
{
    class Program
    {
        static void Main(string[] args)
        {
            InteractiveProcess service = new InteractiveProcess();

            if (args.Length > 0 && args[0] == "/i")
            {
                //Launched interactively.
                service.LaunchService(null);
            }
            else
            {
                //Standard service entry point.
                ServiceBase.Run(service);
            }
        }
    }
}
