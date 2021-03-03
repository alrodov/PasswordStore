namespace PasswordStore.Lib.Implementation
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using PasswordStore.Lib.Crypto;
    using PasswordStore.Lib.Entities;
    using PasswordStore.Lib.Interfaces;

    public class AuthService : IAuthService
    {
        private IUserIdentity userIdentity;

        private IDataStore<User> userStore;
        
        public AuthService(IUserIdentity userIdentity, IDataStore<User> userStore)
        {
            this.userIdentity = userIdentity;
            this.userStore = userStore;
        }
        
        public void Login(string userName, string masterKey)
        {
            var user = this.userStore.GetAll().FirstOrDefault(x => x.Login == userName);
            if (user == null)
            {
                this.ThrowAuthError();
            }
            
            var keyHash = CryptographyUtils.GetKeyHash(masterKey);
            if (keyHash != user.MasterKey)
            {
                this.ThrowAuthError();
            }
            
            this.userIdentity.SetIdentity(user.Id, masterKey);
        }

        public void Logout()
        {
            this.userIdentity.ResetIdentity();
        }

        private void ThrowAuthError()
        {
            throw new Exception("Невозможно выполнить вход. Указан неверный логин или ключ доступа.");
        }
    }
}