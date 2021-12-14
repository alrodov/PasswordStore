namespace PasswordStore.CLI
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using NLog;
    using PasswordStore.AppStarter;
    using PasswordStore.Core;

    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
            try
            {
                AppStarter.StartApp(config);
                logger.Info("Приложение запущено");
                var app = new ApplicationLifecycle(Application.ServiceProvider);
                await app.RunAsync();
            }
            catch (Exception e)
            {
                logger.Error(e, e.Message);
            }
        }
    }
}