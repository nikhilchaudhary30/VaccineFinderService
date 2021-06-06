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
                    s.ConstructUsing(service => new ServiceMethods());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });

                x.RunAsLocalSystem();

                x.SetServiceName("TelegramBotService");
                x.SetDisplayName("Telegram Bot Service");
                x.SetDescription("This is the Telegram Bot Notification Service.");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }        
    }
}
