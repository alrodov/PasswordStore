namespace PasswordStore.AppStarter
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NLog;
    using NLog.Extensions.Logging;
    using PasswordStore.Core;
    using PasswordStore.Core.Interfaces;

    /// <summary>
    /// Класс - инициализатор приложения
    /// </summary>
    public static class AppStarter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        private static bool isInitialized = false;

        private static object lockObj = new object();

        private static Type[] AppModules =
        {
            typeof(Lib.Module)
        };

        /// <summary>
        /// Метод инициализации приложения
        /// </summary>
        public static void StartApp(IConfiguration config)
        {
            if (!isInitialized)
            {
                lock (lockObj)
                {
                    if (!isInitialized)
                    {
                        logger.Info("Выполняется запуск приложения");
                        var serviceCollection = new ServiceCollection();
                        serviceCollection.AddLogging(loggingBuilder =>
                        {
                            loggingBuilder.ClearProviders();
                            loggingBuilder.AddNLog(config);
                        });

                        serviceCollection.AddSingleton(config);
                        
                        logger.Info("Выполняется инициализация модулей");
                        foreach (var moduleType in AppModules)
                        {
                            var instance = (IAppModule)Activator.CreateInstance(moduleType);
                            instance!.Initialize(serviceCollection, config);
                        }
                        
                        logger.Info("Модули приложения проинициализированы");
                        var serviceProvider = serviceCollection.BuildServiceProvider();
                        Application.SetServiceCollection(serviceProvider);
                        
                        isInitialized = true;
                    }
                }
            }
        }
    }
}