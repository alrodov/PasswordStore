namespace PasswordStore.Lib.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using PasswordStore.Lib.Data;
    using PasswordStore.Lib.Entities;
    using PasswordStore.Lib.Interfaces;

    public class DataStore<TEntity> : IDataStore<TEntity>, IDisposable where TEntity : BaseEntity, new()
    {
        protected DataContext dataContext;

        protected DbSet<TEntity> dbSet;
    
        public DataStore(DataContext dataContext)
        {
            this.dataContext = dataContext;
            this.dbSet = dataContext.Set<TEntity>();
        }
        
        public virtual IQueryable<TEntity> GetAll()
        {
            return this.dbSet.AsQueryable();
        }

        public virtual TEntity Get(long id)
        {
            return this.dbSet.SingleOrDefault(x => x.Id == id);
        }

        public void Create(TEntity entity)
        {
            this.Create(new [] { entity });
        }

        public virtual void Create(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                this.dbSet.Add(entity);
            }

            this.dataContext.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            this.Update(new [] { entity });
        }

        public virtual void Update(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                this.dataContext.Entry(entity).State = EntityState.Modified;
            }
            
            this.dataContext.SaveChanges();
        }

        public void Delete(long entityId)
        {
            this.Delete(new [] { entityId });
        }

        public virtual void Delete(IEnumerable<long> entityIds)
        {
            this.dbSet.RemoveRange(this.GetAll().Where(x => entityIds.Contains(x.Id)));
            this.dataContext.SaveChanges();
        }

        public void Dispose()
        {
            this.dataContext.Dispose();
        }
    }
}