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

        TEntity Get(long id);

        void Create(TEntity entity);

        void Create(IEnumerable<TEntity> entities);
        
        void Update(TEntity entity);

        void Update(IEnumerable<TEntity> entities);

        void Delete(long entityId);

        void Delete(IEnumerable<long> entityIds);
    }
}