namespace PasswordStore.Lib.Interfaces
{
    using System.Collections.Generic;
    using PasswordStore.Lib.Entities;

    /// <summary>
    /// Сервис для работы с учётными данными
    /// </summary>
    public interface ICredentialService
    {
        /// <summary>
        /// Возвращает список паролей
        /// </summary>
        IList<Credential> ListAllCredentials();
        
        /// <summary>
        /// Поиск паролей
        /// </summary>
        /// <param name="name">Имя сервиса</param>
        /// <param name="matchExactly">Признак выполнения по полному совпадению имени</param>
        /// <returns>Список найденных паролей</returns>
        IList<Credential> FindByName(string name, bool matchExactly);

        /// <summary>
        /// Добавляет новую запись учётных данных
        /// </summary>
        /// <param name="serviceName">Имя сервиса, к которому относится пароль</param>
        /// <param name="login">Логин учётной записи</param>
        /// <param name="password">Пароль (в открытом виде)</param>
        void AddCredential(string serviceName, string login, string password);

        /// <summary>
        /// Изменяет значение пароля
        /// </summary>
        /// <param name="serviceName">Имя сервиса, к которому относится пароль</param>
        /// <param name="login">Логин учётной записи</param>
        /// <param name="password">Пароль (в открытом виде)</param>
        void ChangePassword(string serviceName, string login, string password);

        /// <summary>
        /// Удаляет пароль
        /// </summary>
        /// <param name="serviceName">Имя сервиса, к которому относится пароль</param>
        /// <param name="login">Логин учётной записи, от которой удаляется пароль</param>
        void RemoveCredential(string serviceName, string login = null);

        /// <summary>
        /// Изменяет ключ доступа пользователя с выполнением перешифровки всех паролей
        /// </summary>
        /// <param name="newKey">Новое значение ключа доступа</param>
        void ChangeMasterKey(string newKey);

        /// <summary>
        /// Возвращает пароль в открытом виде
        /// </summary>
        /// <param name="serviceName">Имя сервиса, к которому относится пароль</param>
        /// <param name="login">Логин учётной записи</param>
        string ShowPassword(string serviceName, string login = null);
    }
}