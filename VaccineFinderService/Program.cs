using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace VaccineFinderService
{
    class Program
    {
        static void Main(string[] args)
        {
            //ServiceMethods.APIInitiator_3(); -//-- For testing purpose uncomment this line of code.
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<ServiceMethods>(s =>
                {
                    s.ConstructUsing(vaccine => new ServiceMethods());
                    s.WhenStarted(vaccine => vaccine.Start());
                    s.WhenStopped(vaccine => vaccine.Stop());
                });

                x.RunAsLocalSystem();

                x.SetServiceName("VaccineNotificationService");
                x.SetDisplayName("Vaccine Notification Service");
                x.SetDescription("This is the Vaccine Notification Service.");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
