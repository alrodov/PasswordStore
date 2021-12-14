namespace PasswordStore.Lib.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
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

        public virtual async Task<TEntity> GetAsync(long id, CancellationToken cancellationToken = default)
        {
            return await this.dbSet.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await this.CreateAsync(new [] { entity }, cancellationToken);
        }

        public virtual async Task CreateAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await this.dbSet.AddRangeAsync(entities, cancellationToken);
            await this.dataContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await this.UpdateAsync(new [] { entity }, cancellationToken);
        }

        public virtual async Task UpdateAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                this.dataContext.Entry(entity).State = EntityState.Modified;
            }
            
            await this.dataContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(long entityId, CancellationToken cancellationToken = default)
        {
            await this.DeleteAsync(new [] { entityId }, cancellationToken);
        }

        public virtual async Task DeleteAsync(ICollection<long> entityIds, CancellationToken cancellationToken = default)
        {
            this.dbSet.RemoveRange(this.GetAll().Where(x => entityIds.Contains(x.Id)));
            await this.dataContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            this.dataContext.Dispose();
        }
    }
}