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
        private IDataStore<SecretQuestion> questionStore;

        private IUserIdentity userIdentity;

        private ITransactionManager transactionManager;
        
        public CredentialService(
            ITransactionManager transactionManager,
            IDataStore<Credential> credentialStore,
            IDataStore<User> userStore,
            IDataStore<SecretQuestion> questionStore,
            IUserIdentity userIdentity)
        {
            this.transactionManager = transactionManager;
            this.userIdentity = userIdentity;
            this.credentialStore = credentialStore;
            this.questionStore = questionStore;
            this.userStore = userStore;
        }
    
        public async Task<IList<Credential>> ListCredentialsAsync(string filterValue, CancellationToken cancellationToken = default)
        {
            var query = this.credentialStore.GetAll().Include(x => x.SecretQuestions).AsQueryable();
            if (!string.IsNullOrEmpty(filterValue))
            {
                var searchPattern = $"%{filterValue.ToLower()}%";
                query = query.Where(item =>
                    EF.Functions.Like(item.Login.ToLower(), searchPattern) || EF.Functions.Like(item.ServiceName.ToLower(), searchPattern));
            }
            
            return await query.ToListAsync(cancellationToken);
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
            await InternalAdd(new Credential
            {
                Login = login,
                ServiceName = serviceName,
                OpenPassword = password
            }, cancellationToken);
        }

        public async Task AddCredentialAsync(Credential credential, CancellationToken cancellationToken = default)
        {
            await InternalAdd(credential, cancellationToken);
        }

        public async Task EditCredentialAsync(long id, string serviceName, string login, string password,
            CancellationToken cancellationToken = default)
        {
            await InternalEdit(new Credential
            {
                Id = id,
                ServiceName = serviceName,
                Login = login,
                OpenPassword = password
            }, cancellationToken);
        }

        public async Task EditCredentialAsync(Credential credential, CancellationToken cancellationToken = default)
        {
            await InternalEdit(credential, cancellationToken);
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

        public async Task RemoveCredentialAsync(long id, CancellationToken cancellationToken = default)
        {
            await this.credentialStore.DeleteAsync(id, cancellationToken);
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

        private async Task InternalAdd(Credential credential, CancellationToken cancellationToken)
        {
            var lowerLogin = credential.Login.ToLowerInvariant();
            var existingCredential = await this.FindCredential(credential.ServiceName, lowerLogin, cancellationToken);
            if (existingCredential != null)
            {
                throw new Exception("Пароль для указанных сервиса и логина уже сохранён. Используйте функцию обновления пароля для изменения данных.");
            }

            if (!string.IsNullOrEmpty(credential.OpenPassword))
            {
                var masterKey = this.userIdentity.GetUserKey();
                var storablePassword = CryptographyUtils.Encrypt(masterKey, credential.OpenPassword);
                credential.Password = storablePassword;
            }
            
            var userId = this.userIdentity.GetUserId();
            credential.UserId = userId!.Value;
            credential.CreateDate = DateTime.Now;
            
            await this.credentialStore.CreateAsync(credential, cancellationToken);
        }

        private async Task InternalEdit(Credential credential, CancellationToken cancellationToken)
        {
            var existingCredential = await this.credentialStore.GetAsync(credential.Id, cancellationToken);
            if (existingCredential == null)
            {
                throw new Exception("Переданная запись не найдена");
            }

            if (!string.IsNullOrEmpty(credential.OpenPassword))
            {
                var masterKey = this.userIdentity.GetUserKey();
                var storablePassword = CryptographyUtils.Encrypt(masterKey, credential.OpenPassword);
                existingCredential.Password = storablePassword;
            }
            
            existingCredential.Login = credential.Login;
            existingCredential.ServiceName = credential.ServiceName;
            existingCredential.PhoneNumber = credential.PhoneNumber;
            existingCredential.IsPhoneNumberAuth = credential.IsPhoneNumberAuth;

            var existingQuestionIds = existingCredential.SecretQuestions.ToDictionary(x => x.Id);
            var newQuestions = new List<SecretQuestion>();
            var editedQuestions = new List<SecretQuestion>();
            
            foreach (var question in credential.SecretQuestions)
            {
                if (existingQuestionIds.TryGetValue(question.Id, out var existingQuestion))
                {
                    existingQuestion.Question = question.Question;
                    existingQuestion.Answer = question.Answer;
                    editedQuestions.Add(existingQuestion);
                }
                else
                {
                    question.CredentialId = existingCredential.Id;
                    newQuestions.Add(question);
                }
            }

            await using var transaction = await transactionManager.BeginTransactionAsync(cancellationToken);
            await this.questionStore.CreateAsync(newQuestions, cancellationToken);
            await this.questionStore.UpdateAsync(editedQuestions, cancellationToken);

            var removedQuestionIds = existingCredential.SecretQuestions.Select(x => x.Id)
                .Except(credential.SecretQuestions.Select(x => x.Id)).ToList();

            await this.questionStore.DeleteAsync(removedQuestionIds, cancellationToken);
            await this.credentialStore.UpdateAsync(existingCredential, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
    }
}