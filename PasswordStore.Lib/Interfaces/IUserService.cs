namespace PasswordStore.Lib.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using PasswordStore.Lib.Entities;

    /// <summary>
    /// Сервис, содержащий операции для работы с пользователями
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Поиск пользователя по логину
        /// </summary>
        /// <param name="login">логин</param>
        /// <param name="cancellationToken">Маркер отмены операции</param>
        /// <returns>Найденный пользователь или null, если пользователь с логином не существует</returns>
        Task<User> FindUserAsync(string login, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="key">Ключ доступа</param>
        /// <param name="cancellationToken">Маркер отмены операции</param>
        Task RegisterAsync(string login, string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удаляет пользователя
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="cancellationToken">Маркер отмены операции</param>
        Task RemoveUserAsync(string login, CancellationToken cancellationToken = default);
    }
}