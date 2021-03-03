namespace PasswordStore.Lib.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PasswordStore.Lib.Data;
    using PasswordStore.Lib.Entities;
    using PasswordStore.Lib.Interfaces;

    public class CredentialsStore : DataStore<Credential>
    {
        private IUserIdentity userIdentity;
        
        public CredentialsStore(DataContext dataContext, IUserIdentity userIdentity) : base(dataContext)
        {
            this.userIdentity = userIdentity;
        }

        public override IQueryable<Credential> GetAll()
        {
            var userId = this.userIdentity.GetUserId();
            if (!userId.HasValue)
            {
                return Enumerable.Empty<Credential>().AsQueryable();
            }
            
            return base.GetAll().Where(x => x.UserId == userId);
        }

        public override Credential Get(long id)
        {
            var userId = this.userIdentity.GetUserId();
            if (!userId.HasValue)
            {
                return null;
            }

            return this.dbSet.SingleOrDefault(x => x.Id == id && x.UserId == userId);
        }

        public override void Create(IEnumerable<Credential> entities)
        {
            this.ThrowIfNotAuthorized();
            var credentials = entities.ToList();
            this.ThrowIfNotOwner(credentials);
            
            base.Create(credentials);
        }

        public override void Update(IEnumerable<Credential> entities)
        {
            this.ThrowIfNotAuthorized();
            
            var credentials = entities.ToList();
            this.ThrowIfNotOwner(credentials);
            
            base.Update(credentials);
        }

        public override void Delete(IEnumerable<long> entityIds)
        {
            this.ThrowIfNotAuthorized();

            var idsList = entityIds.ToList();
            var userId = this.userIdentity.GetUserId();
            var hasNotOwnedEntities = this.GetAll()
                .Where(x => idsList.Contains(x.Id))
                .Any(x => x.UserId != userId);

            if (hasNotOwnedEntities)
            {
                throw new Exception("Невозможно выполнить удаление данных. Одна или несколько записей не принадлежит текущему пользователю.");
            }
            
            base.Delete(idsList);
        }

        /// <summary>
        /// Выбросить ошибку, если не выполнен вход в систему
        /// </summary>
        private void ThrowIfNotAuthorized()
        {
            var userId = this.userIdentity.GetUserId();
            if (!userId.HasValue)
            {
                throw new Exception("Невозможно сохранить данные. Не выполнен вход в приложение");
            }
        }

        /// <summary>
        /// Выбросить ошибку, если в данных есть записи, не принадлежащие текущему пользователю
        /// </summary>
        private void ThrowIfNotOwner(IEnumerable<Credential> credentials)
        {
            
            var id = this.userIdentity.GetUserId()!.Value;
            if (credentials.Any(x => x.UserId != id))
            {
                throw new Exception("Невозможно сохранить данные. Одна или несколько записей не принадлежит текущему пользователю.");
            }
        }
    }
}