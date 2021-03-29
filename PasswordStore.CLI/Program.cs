namespace PasswordStore.CLI
{
    using System;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using NLog;
    using PasswordStore.AppStarter;
    using PasswordStore.Core;

    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
            try
            {
                AppStarter.StartApp(config);
                logger.Info("Приложение запущено");
                var app = new ApplicationLifecycle(Application.ServiceProvider);
                app.Run();
            }
            catch (Exception e)
            {
                logger.Error(e, e.Message);
            }
        }
    }
}