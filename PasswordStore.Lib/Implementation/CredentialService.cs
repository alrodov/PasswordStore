namespace PasswordStore.Lib.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using PasswordStore.Lib.Crypto;
    using PasswordStore.Lib.Entities;
    using PasswordStore.Lib.Interfaces;

    public class CredentialService : ICredentialService
    {
        private IDataStore<Credential> credentialStore;
        private IDataStore<User> userStore;

        private IUserIdentity userIdentity;

        private ITransactionManager transactionManager;
        
        public CredentialService(
            ITransactionManager transactionManager,
            IDataStore<Credential> credentialStore,
            IDataStore<User> userStore,
            IUserIdentity userIdentity)
        {
            this.transactionManager = transactionManager;
            this.credentialStore = credentialStore;
            this.userIdentity = userIdentity;
            this.userStore = userStore;
        }
    
        public async Task<IList<Credential>> ListAllCredentialsAsync(CancellationToken cancellationToken = default)
        {
            return await this.credentialStore.GetAll().ToListAsync(cancellationToken);
        }

        public async Task<IList<Credential>> FindByNameAsync(string name, bool matchExactly, CancellationToken cancellationToken = default)
        {
            var query = this.credentialStore.GetAll();
            query = matchExactly
                ? query.Where(x => x.ServiceName == name)
                : query.Where(x => x.ServiceName.Contains(name));
            return await query.ToListAsync(cancellationToken);
        }

        public async Task AddCredentialAsync(string serviceName, string login, string password, CancellationToken cancellationToken = default)
        {
            var lowerLogin = login.ToLowerInvariant();
            var existingCredential = await this.FindCredential(serviceName, lowerLogin, cancellationToken);
            if (existingCredential != null)
            {
                throw new Exception("Пароль для указанных сервиса и логина уже сохранён. Используйте функцию обновления пароля для изменения данных.");
            }
            
            var masterKey = this.userIdentity.GetUserKey();
            var storablePassword = CryptographyUtils.Encrypt(masterKey, password);
            var userId = this.userIdentity.GetUserId();
            await this.credentialStore.CreateAsync(new Credential
            {
                Login = lowerLogin,
                Password = storablePassword,
                ServiceName = serviceName,
                UserId = userId!.Value
            },
            cancellationToken);
        }

        public async Task ChangePasswordAsync(string serviceName, string login, string password, CancellationToken cancellationToken = default)
        {
            var lowerLogin = login.ToLowerInvariant();
            var existingCredential = await this.FindCredential(serviceName, lowerLogin, cancellationToken);
            if (existingCredential == null)
            {
                throw new Exception("Запись для указанных сервиса и логина не найдена. Используйте функцию добавления записи для сохранения пароля.");
            }
            
            var masterKey = this.userIdentity.GetUserKey();
            var storablePassword = CryptographyUtils.Encrypt(masterKey, password);
            existingCredential.Password = storablePassword;
            await this.credentialStore.UpdateAsync(existingCredential, cancellationToken);
        }

        public async Task<string> ShowPasswordAsync(string serviceName, string login = null, CancellationToken cancellationToken = default)
        {
            var credential = await this.credentialStore.GetAll()
                .SingleOrDefaultAsync(x => x.ServiceName == serviceName && x.Login == login, cancellationToken);
            var key = this.userIdentity.GetUserKey();

            return credential != null
                ? CryptographyUtils.Decrypt(key, credential.Password)
                : null;
        }

        public async Task RemoveCredentialAsync(string serviceName, string login = null, CancellationToken cancellationToken = default)
        {
            var idsQuery = this.credentialStore.GetAll()
                .Where(x => x.ServiceName == serviceName);
            if (login != null)
            {
                var loginFilterValue = login.ToLowerInvariant();
                idsQuery = idsQuery.Where(x => x.Login == loginFilterValue);
            }

            var ids = await idsQuery.Select(x => x.Id).ToListAsync(cancellationToken);
            if (!ids.Any())
            {
                throw new Exception("В хранилище не найдены записи по указанным сервису и логину");
            }
            
            await this.credentialStore.DeleteAsync(ids, cancellationToken);
        }

        public async Task ChangeMasterKeyAsync(string newKey, CancellationToken cancellationToken = default)
        {
            var userId = this.userIdentity.GetUserId()!.Value;
            var currentKey = this.userIdentity.GetUserKey();
            await using (var transaction = await this.transactionManager.BeginTransactionAsync(cancellationToken))
            {
                var credentials = credentialStore.GetAll().ToList();
                var user = await this.userStore.GetAsync(userId, cancellationToken);
                user.MasterKey = CryptographyUtils.GetKeyHash(newKey);
                await this.userStore.UpdateAsync(user, cancellationToken);

                foreach (var credential in credentials)
                {
                    credential.Password = CryptographyUtils.Encrypt(newKey,
                        CryptographyUtils.Decrypt(currentKey, credential.Password));
                }
                
                await this.credentialStore.UpdateAsync(credentials, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            
            
            this.userIdentity.SetIdentity(userId, newKey);
        }

        private async Task<Credential> FindCredential(string serviceName, string login, CancellationToken cancellationToken)
        {
            return await this.credentialStore
                .GetAll()
                .SingleOrDefaultAsync(x => x.ServiceName == serviceName && x.Login == login, cancellationToken);
        }
    }
}