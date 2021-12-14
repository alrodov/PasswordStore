namespace PasswordStore.Lib.Interfaces
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using PasswordStore.Lib.Entities;

    /// <summary>
    /// Сервис для работы с учётными данными
    /// </summary>
    public interface ICredentialService
    {
        /// <summary>
        /// Возвращает список паролей
        /// </summary>
        Task<IList<Credential>> ListAllCredentialsAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Поиск паролей
        /// </summary>
        /// <param name="name">Имя сервиса</param>
        /// <param name="matchExactly">Признак выполнения по полному совпадению имени</param>
        /// <param name="cancellationToken">Маркер отмены операции</param>
        /// <returns>Список найденных паролей</returns>
        Task<IList<Credential>> FindByNameAsync(string name, bool matchExactly, CancellationToken cancellationToken = default);

        /// <summary>
        /// Добавляет новую запись учётных данных
        /// </summary>
        /// <param name="serviceName">Имя сервиса, к которому относится пароль</param>
        /// <param name="login">Логин учётной записи</param>
        /// <param name="password">Пароль (в открытом виде)</param>
        /// <param name="cancellationToken">Маркер отмены операции</param>
        Task AddCredentialAsync(string serviceName, string login, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// Изменяет значение пароля
        /// </summary>
        /// <param name="serviceName">Имя сервиса, к которому относится пароль</param>
        /// <param name="login">Логин учётной записи</param>
        /// <param name="password">Пароль (в открытом виде)</param>
        /// <param name="cancellationToken">Маркер отмены операции</param>
        Task ChangePasswordAsync(string serviceName, string login, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удаляет пароль
        /// </summary>
        /// <param name="serviceName">Имя сервиса, к которому относится пароль</param>
        /// <param name="login">Логин учётной записи, от которой удаляется пароль</param>
        /// <param name="cancellationToken">Маркер отмены операции</param>
        Task RemoveCredentialAsync(string serviceName, string login = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Изменяет ключ доступа пользователя с выполнением перешифровки всех паролей
        /// </summary>
        /// <param name="newKey">Новое значение ключа доступа</param>
        /// <param name="cancellationToken">Маркер отмены операции</param>
        Task ChangeMasterKeyAsync(string newKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Возвращает пароль в открытом виде
        /// </summary>
        /// <param name="serviceName">Имя сервиса, к которому относится пароль</param>
        /// <param name="login">Логин учётной записи</param>
        /// <param name="cancellationToken">Маркер отмены операции</param>
        Task<string> ShowPasswordAsync(string serviceName, string login = null, CancellationToken cancellationToken = default);
    }
}