namespace PasswordStore.Lib.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Сервис аутентификации пользователей
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Вход пользователя в систему
        /// </summary>
        /// <param name="userName">Логин</param>
        /// <param name="masterKey">Ключ доступа</param>
        /// <param name="cancellationToken">Маркер отмены операции</param>
        /// <returns>Признак успеха аутентификации</returns>
        Task<bool> LoginAsync(string userName, string masterKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Выход текущего пользователя из системы
        /// </summary>
        void Logout();
    }
}