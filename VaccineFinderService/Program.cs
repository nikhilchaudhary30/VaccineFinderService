using System;
using Topshelf;

namespace VaccineFinderService
{
    class Program
    {
        static void Main(string[] args)
        {
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
