namespace PasswordStore.Lib.Implementation
{
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
        
        public IDbContextTransaction BeginTransaction()
        {
            return this.dataContext.Database.BeginTransaction();
        }
    }
}