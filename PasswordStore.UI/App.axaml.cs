using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using NLog;
using PasswordStore.UI.ViewModels;
using PasswordStore.UI.Views;

namespace PasswordStore.UI
{
    public class App : Application
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public override void Initialize()
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
            try
            {
                PasswordStore.AppStarter.AppStarter.StartApp(config);
                logger.Info("Ядро приложения запущено");
                AvaloniaXamlLoader.Load(this);
                logger.Info("Пользовательское приложение запущено");
            }
            catch (Exception e)
            {
                logger.Error(e, e.Message);
            }
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(PasswordStore.Core.Application.ServiceProvider),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}