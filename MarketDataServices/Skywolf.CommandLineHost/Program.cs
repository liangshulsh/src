using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.ServiceModel;
using System.Diagnostics;
using System.Configuration;
using System.Security;
using Skywolf.Utility;

namespace Skywolf.CommandLineHost
{
    public class Program
    {
        static void Main(string[] args)
        {
            ServiceHost[] svcHosts = null;
            try
            {
                EchoService.StartLoggerNoEmail();

                svcHosts = new ServiceHost[]
                {
                    new ServiceHost( typeof(Skywolf.MarketDataService.MarketDataService))
                };

                foreach (ServiceHost host in svcHosts)
                {
                    host.Open();
                    Console.WriteLine("Serivce: {0} \t\t State:{1}", host.Description.Name, host.State);
                }

                // ModelBase.InitHolidaySchedule();

                Console.WriteLine();
                Console.WriteLine("Press <ENTER> to terminate Host");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
                EchoService.Logger.Error(string.Format("Exception thrown from {0}", "unknown"), ex);
            }
            finally
            {
                if (svcHosts != null)
                {
                    foreach (ServiceHost host in svcHosts)
                    {
                        if (host.State == CommunicationState.Faulted)
                            host.Abort();
                        else
                            host.Close();
                    }
                }
                //Prcm.Services.Models.ModelBase.UnInitHolidaySchedule();

                //EchoService.DeleteLog();
            }
        }
    }
}