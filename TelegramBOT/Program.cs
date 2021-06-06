using System;
using Telegram.Bot;
using Topshelf;

namespace TelegramBOT
{
    class Program
    {
        static void Main(string[] args)
        {
            //ServiceMethods.TelegramBotCovidInfoAPIInitiator(0,null,null);
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<ServiceMethods>(s =>
                {
                    s.ConstructUsing(vaccine => new ServiceMethods());
                    s.WhenStarted(vaccine => vaccine.Start());
                    s.WhenStopped(vaccine => vaccine.Stop());
                });

                x.RunAsLocalSystem();

                x.SetServiceName("TelegramBotService");
                x.SetDisplayName("Telegram Bot Service");
                x.SetDescription("This is the TelegramBot Vaccine Notification Service.");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }        
    }
}
