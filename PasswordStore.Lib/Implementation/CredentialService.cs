namespace PasswordStore.Lib.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    
        public IList<Credential> ListAllCredentials()
        {
            return this.credentialStore.GetAll().ToList();
        }

        public IList<Credential> FindByName(string name, bool matchExactly)
        {
            var query = this.credentialStore.GetAll();
            query = matchExactly
                ? query.Where(x => x.ServiceName == name)
                : query.Where(x => x.ServiceName.Contains(name));
            return query.ToList();
        }

        public void AddCredential(string serviceName, string login, string password)
        {
            var lowerLogin = login.ToLowerInvariant();
            var existingCredential = this.FindCredential(serviceName, lowerLogin);
            if (existingCredential != null)
            {
                throw new Exception("Пароль для указанных сервиса и логина уже сохранён. Используйте функцию обновления пароля для изменения данных.");
            }
            
            var masterKey = this.userIdentity.GetUserKey();
            var storablePassword = CryptographyUtils.Encrypt(masterKey, password);
            var userId = this.userIdentity.GetUserId();
            this.credentialStore.Create(new Credential
            {
                Login = lowerLogin,
                Password = storablePassword,
                ServiceName = serviceName,
                UserId = userId.Value
            });
        }

        public void ChangePassword(string serviceName, string login, string password)
        {
            var lowerLogin = login.ToLowerInvariant();
            var existingCredential = this.FindCredential(serviceName, lowerLogin);
            if (existingCredential == null)
            {
                throw new Exception("Запись для указанных сервиса и логина не найдена. Используйте функцию добавления записи для сохранения пароля.");
            }
            
            var masterKey = this.userIdentity.GetUserKey();
            var storablePassword = CryptographyUtils.Encrypt(masterKey, password);
            existingCredential.Password = storablePassword;
            this.credentialStore.Update(existingCredential);
        }

        public string ShowPassword(string serviceName, string login = null)
        {
            var credential = this.credentialStore.GetAll()
                .SingleOrDefault(x => x.ServiceName == serviceName && x.Login == login);
            var key = this.userIdentity.GetUserKey();

            return credential != null
                ? CryptographyUtils.Decrypt(key, credential.Password)
                : null;
        }

        public void RemoveCredential(string serviceName, string login = null)
        {
            var idsQuery = this.credentialStore.GetAll()
                .Where(x => x.ServiceName == serviceName);
            if (login != null)
            {
                var loginFilterValue = login.ToLowerInvariant();
                idsQuery = idsQuery.Where(x => x.Login == loginFilterValue);
            }

            var ids = idsQuery.Select(x => x.Id).ToList();
            if (!ids.Any())
            {
                throw new Exception("В хранилище не найдены записи по указанным сервису и логину");
            }
            
            this.credentialStore.Delete(ids);
        }

        public void ChangeMasterKey(string newKey)
        {
            var userId = this.userIdentity.GetUserId().Value;
            var currentKey = this.userIdentity.GetUserKey();
            using (var transaction = this.transactionManager.BeginTransaction())
            {
                var credentials = credentialStore.GetAll().ToList();
                var user = this.userStore.Get(userId);
                user.MasterKey = CryptographyUtils.GetKeyHash(newKey);
                this.userStore.Update(user);

                foreach (var credential in credentials)
                {
                    credential.Password = CryptographyUtils.Encrypt(newKey,
                        CryptographyUtils.Decrypt(currentKey, credential.Password));
                }
                
                this.credentialStore.Update(credentials);
                transaction.Commit();
            }
            
            
            this.userIdentity.SetIdentity(userId, newKey);
        }

        private Credential FindCredential(string serviceName, string login)
        {
            return this.credentialStore
                .GetAll()
                .SingleOrDefault(x => x.ServiceName == serviceName && x.Login == login);
        }
    }
}