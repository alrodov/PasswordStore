namespace PasswordStore.Lib.Implementation
{
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using PasswordStore.Lib.Data;
    using PasswordStore.Lib.Interfaces;

    public class TransactionManager : ITransactionManager
    {
        protected DataContext dataContext;
    
        public TransactionManager(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken,
            System.Data.IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return await this.dataContext.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        }
    }
}