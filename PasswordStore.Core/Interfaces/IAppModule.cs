namespace PasswordStore.Core.Interfaces
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Модуль приложения
    /// </summary>
    public interface IAppModule
    {
        /// <summary> 
        /// Проинициализировать модуль с регистрацией компонентов в DI-контейнере
        /// </summary>
        /// <param name="serviceCollection">Коллекция сервисов для регистрации компонентов</param>
        /// <param name="configuration">Конфигурация приложения</param>
        void Initialize(IServiceCollection serviceCollection, IConfiguration configuration);
    }
}