namespace PasswordStore.Lib
{
    using System;
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using PasswordStore.Core.Interfaces;
    using PasswordStore.Lib.Data;
    using PasswordStore.Lib.Entities;
    using PasswordStore.Lib.Implementation;
    using PasswordStore.Lib.Interfaces;

    public class Module : IAppModule
    {
        public void Initialize(IServiceCollection serviceCollection)
        {
            this.InitDatabase();
            this.RegisterDbContext(serviceCollection);
            
            serviceCollection.AddSingleton<IUserIdentity, UserIdentity>();
            serviceCollection.AddScoped<IDataStore<User>, DataStore<User>>();
            serviceCollection.AddScoped<IDataStore<Credential>, CredentialsStore>();
            serviceCollection.AddScoped<ICredentialService, CredentialService>();
            serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddScoped<IAuthService, AuthService>();
        }

        private void InitDatabase()
        {
            var serviceCollection = new ServiceCollection();
            this.RegisterDbContext(serviceCollection);

            using (var scope = serviceCollection.BuildServiceProvider().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<DataContext>();
                context!.Database.Migrate();
            }
        }

        private void RegisterDbContext(IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<DataContext>(builder => builder.UseSqlite(this.GetConnectionString()));
        }

        private string GetConnectionString()
        {
            // TODO вынести в конфиг
            var directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PasswordStore");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            
            var databasePath = Path.Combine(directoryPath, "data.db");
            
            return $"Data Source = {databasePath}";
        }
    }
}