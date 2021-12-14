namespace PasswordStore.Lib.Implementation
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using PasswordStore.Lib.Crypto;
    using PasswordStore.Lib.Entities;
    using PasswordStore.Lib.Interfaces;

    public class UserService : IUserService
    {
        private IDataStore<User> userStore;
        
        private IDataStore<Credential> credentialStore;

        private ITransactionManager transactionManager;

        public UserService(IDataStore<User> userStore, IDataStore<Credential> credentialStore, ITransactionManager transactionManager)
        {
            this.userStore = userStore;
            this.credentialStore = credentialStore;
            this.transactionManager = transactionManager;
        }

        public async Task<User> FindUserAsync(string login, CancellationToken cancellationToken = default)
        {
            return await this.userStore.GetAll().FirstOrDefaultAsync(x => x.Login == login, cancellationToken);
        }

        public async Task RegisterAsync(string login, string key, CancellationToken cancellationToken = default)
        {
            await using var transaction = await transactionManager.BeginTransactionAsync(cancellationToken);
            if (this.userStore.GetAll().Any(x => x.Login == login))
            {
                throw new Exception($"Пользователь с логином {login} уже существует");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new Exception($"Ключ доступа пользователя не может быть пустым");
            }
            
            await this.userStore.CreateAsync(new User
            {
                Login = login,
                MasterKey = CryptographyUtils.GetKeyHash(key)
            },
            cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
        }

        public async Task RemoveUserAsync(string login, CancellationToken cancellationToken = default)
        {
            await using var transaction = await transactionManager.BeginTransactionAsync(cancellationToken);
            var user = this.userStore.GetAll().SingleOrDefault(x => x.Login == login);
            if (user == null)
            {
                throw new Exception($"Пользователь с логином {login} не найден");
            }
            
            var credentials = this.credentialStore.GetAll().Where(x => x.UserId == user.Id);
            await this.credentialStore.DeleteAsync(credentials.Select(x => x.Id).ToList(), cancellationToken);
            
            await this.userStore.DeleteAsync(user.Id, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
    }
}