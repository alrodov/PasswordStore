namespace PasswordStore.Lib.Interfaces
{
    using Microsoft.EntityFrameworkCore.Storage;

    /// <summary>
    /// Компонент для управления транзакциями БД
    /// </summary>
    public interface ITransactionManager
    {
        /// <summary>
        /// Открыть транзакцию
        /// </summary>
        /// <returns>Транзакция</returns>
        IDbContextTransaction BeginTransaction();
    }
}