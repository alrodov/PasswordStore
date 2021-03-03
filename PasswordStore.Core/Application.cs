namespace PasswordStore.Core
{
    using System;

    /// <summary>
    /// Статический класс, описывающий глобальные объекты приложения
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// Контейнер DI
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Сохранить ссылку на контейнер
        /// </summary>
        /// <param name="serviceProvider">Контейнер DI</param>
        public static void SetServiceCollection(IServiceProvider serviceProvider)
        {
            if (ServiceProvider == null)
            {
                ServiceProvider = serviceProvider;
            }
        }
    }
}