namespace PasswordStore.UI.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using PasswordStore.Lib.Crypto;
    using PasswordStore.Lib.Interfaces;
    using PasswordStore.UI.Models;
    using ReactiveUI;

    public class PasswordGridViewModel: ViewModelBase
    {
        private readonly ICredentialService credentialService;
        private readonly IUserIdentity userIdentity;
        private ICollection<CredentialModel> credentials;

        public PasswordGridViewModel(IServiceProvider serviceProvider)
        {
            this.credentialService = serviceProvider.GetRequiredService<ICredentialService>();
            this.userIdentity = serviceProvider.GetRequiredService<IUserIdentity>();
            this.InitCommands();
            
            this.LoadData();
        }

        public ICollection<CredentialModel> Credentials
        {
            get => credentials;
            set => credentials = this.RaiseAndSetIfChanged(ref credentials, value);
        }

        public ReactiveCommand<Unit, Unit> Edit { get; private set; }
        
        public ReactiveCommand<Unit, Unit> Remove { get; private set; }

        private void InitCommands()
        {
            this.Edit = ReactiveCommand.CreateFromTask(DoEdit);
            this.Remove = ReactiveCommand.CreateFromTask(DoRemove);
        }

        private async Task LoadData()
        {
            var key = this.userIdentity.GetUserKey();
            var cts = new CancellationTokenSource(App.DefaultTimeoutMilliseconds);
            var data = await credentialService.ListAllCredentialsAsync(cts.Token);
            this.Credentials = data.Select(cr => new CredentialModel
            {
                Id = cr.Id,
                ServiceName = cr.ServiceName,
                Login = cr.Login,
                Password = CryptographyUtils.Decrypt(key, cr.Password)
            }).ToList();
        }

        public async Task DoEdit()
        {
            
        }

        public async Task DoRemove()
        {
            
        }
    }
}