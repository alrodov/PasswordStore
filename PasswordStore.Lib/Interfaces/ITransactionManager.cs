namespace PasswordStore.Lib.Interfaces
{
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Storage;

    /// <summary>
    /// Компонент для управления транзакциями БД
    /// </summary>
    public interface ITransactionManager
    {
        /// <summary>
        /// Открыть транзакцию
        /// </summary>
        /// <param name="cancellationToken">Маркер отмены операции</param>
        /// <param name="isolationLevel">Уровень изоляции транзакции</param>
        /// <returns>Транзакция</returns>
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}