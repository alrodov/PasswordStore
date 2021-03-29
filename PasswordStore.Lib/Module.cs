namespace PasswordStore.Lib
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using NLog;
    using PasswordStore.Core.Interfaces;
    using PasswordStore.Lib.Data;
    using PasswordStore.Lib.Entities;
    using PasswordStore.Lib.Implementation;
    using PasswordStore.Lib.Interfaces;

    public class Module : IAppModule
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        public void Initialize(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            this.InitDatabase(configuration);
            this.RegisterDbContext(serviceCollection, configuration);
            
            serviceCollection.AddSingleton<IUserIdentity, UserIdentity>();
            serviceCollection.AddScoped<ITransactionManager, TransactionManager>();
            serviceCollection.AddScoped(typeof(IDataStore<>), typeof(DataStore<>));
            serviceCollection.AddScoped<IDataStore<Credential>, CredentialsStore>();
            serviceCollection.AddScoped<ICredentialService, CredentialService>();
            serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddScoped<IAuthService, AuthService>();
        }

        private void InitDatabase(IConfiguration configuration)
        {
            var serviceCollection = new ServiceCollection();
            this.RegisterDbContext(serviceCollection, configuration);

            logger.Info("Выполняется проверка инициализации Базы Данных");
            using (var scope = serviceCollection.BuildServiceProvider().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<DataContext>();
                context!.Database.Migrate();
            }
            logger.Info("База данных подключена, все миграции проведены");
        }

        private void RegisterDbContext(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("Database")["ConnectionString"];
            serviceCollection.AddDbContext<DataContext>(builder => builder.UseSqlite(connectionString));
        }
    }
}