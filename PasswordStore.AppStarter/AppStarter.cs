namespace PasswordStore.AppStarter
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using PasswordStore.Core;
    using PasswordStore.Core.Interfaces;

    /// <summary>
    /// Класс - инициализатор приложения
    /// </summary>
    public static class AppStarter
    {
        private static bool isInitialized = false;

        private static object lockObj = new object();

        private static Type[] AppModules =
        {
            typeof(Lib.Module)
        };

        /// <summary>
        /// Метод инициализации приложения
        /// </summary>
        public static void StartApp()
        {
            if (!isInitialized)
            {
                lock (lockObj)
                {
                    if (!isInitialized)
                    {
                        var serviceCollection = new ServiceCollection();
                        foreach (var moduleType in AppModules)
                        {
                            var instance = (IAppModule)Activator.CreateInstance(moduleType);
                            instance!.Initialize(serviceCollection);
                        }

                        var serviceProvider = serviceCollection.BuildServiceProvider();
                        Application.SetServiceCollection(serviceProvider);
                        
                        isInitialized = true;
                    }
                }
            }
        }
    }
}