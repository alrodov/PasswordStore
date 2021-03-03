namespace PasswordStore.Lib.Implementation
{
    using System;
    using System.Linq;
    using PasswordStore.Core;
    using PasswordStore.Lib.Crypto;
    using PasswordStore.Lib.Entities;
    using PasswordStore.Lib.Interfaces;

    public class UserService : IUserService
    {
        private IDataStore<User> userStore;
        
        private IDataStore<Credential> credentialStore;

        public UserService(IDataStore<User> userStore, IDataStore<Credential> credentialStore)
        {
            this.userStore = userStore;
            this.credentialStore = credentialStore;
        }

        public User FindUser(string login)
        {
            return this.userStore.GetAll().FirstOrDefault(x => x.Login == login);
        }

        public void Register(string login, string key)
        {
            // TODO transaction
            if (this.userStore.GetAll().Any(x => x.Login == login))
            {
                throw new Exception($"Пользователь с логином {login} уже существует");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new Exception($"Ключ доступа пользователя не может быть пустым");
            }
            
            this.userStore.Create(new User
            {
                Login = login,
                MasterKey = CryptographyUtils.GetKeyHash(key)
            });
        }

        public void RemoveUser(string login)
        {
            // TODO transaction
            var user = this.userStore.GetAll().SingleOrDefault(x => x.Login == login);
            if (user == null)
            {
                throw new Exception($"Пользователь с логином {login} не найден");
            }
            
            var credentials = this.credentialStore.GetAll().Where(x => x.UserId == user.Id);
            this.credentialStore.Delete(credentials.Select(x => x.Id));
            
            this.userStore.Delete(user.Id);
        }
    }
}