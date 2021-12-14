using System.Threading;
using System.Threading.Tasks;

namespace PasswordStore.Lib.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore.Storage;
    using PasswordStore.Lib.Entities;

    /// <summary>
    /// Хранилище данных для выполнения CRUD-операций над данными
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности хранилища</typeparam>
    public interface IDataStore<TEntity> where TEntity: BaseEntity, new()
    {
        IQueryable<TEntity> GetAll();

        Task<TEntity> GetAsync(long id, CancellationToken cancellationToken = default);

        Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task CreateAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default);
        
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task UpdateAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default);

        Task DeleteAsync(long entityId, CancellationToken cancellationToken = default);

        Task DeleteAsync(ICollection<long> entityIds, CancellationToken cancellationToken = default);
    }
}