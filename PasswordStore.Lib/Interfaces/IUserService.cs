namespace PasswordStore.Lib.Interfaces
{
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
        /// <returns>Найденный пользователь или null, если пользователь с логином не существует</returns>
        User FindUser(string login);
        
        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="key">Ключ доступа</param>
        void Register(string login, string key);

        /// <summary>
        /// Удаляет пользователя
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        void RemoveUser(string login);
    }
}