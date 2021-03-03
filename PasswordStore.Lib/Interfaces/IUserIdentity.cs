namespace PasswordStore.Lib.Interfaces
{
    /// <summary>
    /// Описание активного пользователя
    /// </summary>
    public interface IUserIdentity
    {
        /// <summary>
        /// Возвращает идентификатор активного пользователя или null, если вход в приложение не выполнен
        /// </summary>
        public long? GetUserId();
        
        /// <summary>
        /// Возвращает ключ доступа текущего пользователя
        /// </summary>
        public string GetUserKey();

        /// <summary>
        /// Устанавливает данные активного пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="key">Ключ доступа пользователя</param>
        public void SetIdentity(long userId, string key);

        /// <summary>
        /// Сбрасывает данные активного пользователя
        /// </summary>
        public void ResetIdentity();
    }
}