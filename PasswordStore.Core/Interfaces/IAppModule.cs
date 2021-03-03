namespace PasswordStore.Core.Interfaces
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Модуль приложения
    /// </summary>
    public interface IAppModule
    {
        /// <summary>
        /// Проинициализировать модуль с регистрацией компонентов в DI-контейнере
        /// </summary>
        void Initialize(IServiceCollection serviceCollection);
    }
}