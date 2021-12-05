namespace PasswordStore.Lib.Interfaces
{
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
        /// <returns>Признак успеха аутентификации</returns>
        bool Login(string userName, string masterKey);

        /// <summary>
        /// Выход текущего пользователя из системы
        /// </summary>
        void Logout();
    }
}