namespace PasswordStore.Lib.Implementation
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
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
        
        public async Task<bool> LoginAsync(string userName, string masterKey, CancellationToken cancellationToken = default)
        {
            var user = await this.userStore.GetAll().FirstOrDefaultAsync(x => x.Login == userName, cancellationToken);
            if (user == null)
            {
                this.ThrowAuthError();
            }
            
            var keyHash = CryptographyUtils.GetKeyHash(masterKey);
            if (keyHash != user!.MasterKey)
            {
                this.ThrowAuthError();
            }
            
            this.userIdentity.SetIdentity(user.Id, masterKey);

            return true;
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