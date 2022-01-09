namespace PasswordStore.UI.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.Primitives;
    using Microsoft.Extensions.DependencyInjection;
    using PasswordStore.Lib.Crypto;
    using PasswordStore.Lib.Interfaces;
    using PasswordStore.UI.Models;
    using PasswordStore.UI.Views;
    using ReactiveUI;

    public class PasswordGridViewModel: ViewModelBase
    {
        private readonly ICredentialService credentialService;
        private readonly IUserIdentity userIdentity;
        private ICollection<CredentialModel> credentials;
        private object selectedItem;

        public PasswordGridViewModel(IServiceProvider serviceProvider)
        {
            this.credentialService = serviceProvider.GetRequiredService<ICredentialService>();
            this.userIdentity = serviceProvider.GetRequiredService<IUserIdentity>();
            this.InitCommands();
            
            this.DoLoadData();
        }

        public ICollection<CredentialModel> Credentials
        {
            get => credentials;
            set => this.RaiseAndSetIfChanged(ref credentials, value);
        }

        public object SelectedItem
        {
            get => selectedItem;
            set => this.RaiseAndSetIfChanged(ref selectedItem, value);
        }

        public ReactiveCommand<Unit, Unit> LoadData { get; private set; }
        
        public ReactiveCommand<CredentialModel, Unit> ShowPassword { get; private set; }
        
        public ReactiveCommand<CredentialModel, Unit> CopyToClipboard { get; private set; }

        public ReactiveCommand<Unit, Unit> Add { get; private set; }
        
        public ReactiveCommand<CredentialModel, Unit> Edit { get; private set; }
        
        public ReactiveCommand<CredentialModel, Unit> Remove { get; private set; }
        
        public Interaction<CredentialFormViewModel, CredentialModel> AddCredential { get; set; }
        
        public Interaction<CredentialFormViewModel, CredentialModel> EditCredential { get; set; }

        private void InitCommands()
        {
            this.LoadData = ReactiveCommand.CreateFromTask(DoLoadData);
            this.ShowPassword = ReactiveCommand.CreateFromTask<CredentialModel>(DoShowPassword);
            this.CopyToClipboard = ReactiveCommand.CreateFromTask<CredentialModel>(DoCopyPasswordToClipboard);
            this.Add = ReactiveCommand.CreateFromTask(DoAdd);
            this.Edit = ReactiveCommand.CreateFromTask<CredentialModel>(DoEdit);
            this.Remove = ReactiveCommand.CreateFromTask<CredentialModel>(DoRemove);

            this.EditCredential = new Interaction<CredentialFormViewModel, CredentialModel>();
            this.AddCredential = new Interaction<CredentialFormViewModel, CredentialModel>();
        }

        private async Task DoLoadData()
        {
            // TODO show mask
            var cts = new CancellationTokenSource(App.DefaultTimeoutMilliseconds);
            var data = await credentialService.ListAllCredentialsAsync(cts.Token);
            this.Credentials = data.Select(cr => new CredentialModel
            {
                Id = cr.Id,
                ServiceName = cr.ServiceName,
                Login = cr.Login,
                Password = cr.Password
            }).ToList();
        }

        private async Task DoEdit(CredentialModel record)
        {
            if (string.IsNullOrWhiteSpace(record.OpenPassword))
            {
                record.OpenPassword = CryptographyUtils.Decrypt(userIdentity.GetUserKey(), record.Password);
            }
            
            var editedData = await EditCredential.Handle(new CredentialFormViewModel
            {
                DataOperationName = "Редактирование данных",
                Data = record
            }).FirstOrDefaultAsync();

            if (editedData != null)
            {
                // TODO show mask
                try
                {
                    var cts = new CancellationTokenSource(App.DefaultTimeoutMilliseconds);
                    await this.credentialService.EditCredentialAsync(editedData.Id, editedData.ServiceName,
                        editedData.Login, editedData.OpenPassword, cts.Token);
                    await this.DoLoadData();
                }
                catch (Exception e)
                {
                    await this.ShowMessageWindow($"Произошла ошибка при сохранении данных: {e.Message}");
                }
            }
        }

        private async Task DoAdd()
        {
            var addedData = await AddCredential.Handle(new CredentialFormViewModel
            {
                DataOperationName = "Добавление данных",
                Data = new CredentialModel()
            }).FirstOrDefaultAsync();

            if (addedData != null)
            {
                try
                {
                    // TODO show mask
                    var cts = new CancellationTokenSource(App.DefaultTimeoutMilliseconds);
                    await this.credentialService.AddCredentialAsync(addedData.ServiceName,
                        addedData.Login, addedData.OpenPassword, cts.Token);
                    await this.DoLoadData();
                }
                catch (Exception e)
                {
                    await this.ShowMessageWindow($"Произошла ошибка при сохранении данных: {e.Message}");
                }
            }
        }

        private async Task DoRemove(CredentialModel record)
        {
            
        }

        private async Task DoCopyPasswordToClipboard(CredentialModel record)
        {
            var password = CryptographyUtils.Decrypt(userIdentity.GetUserKey(), record.Password);
            await Application.Current.Clipboard.SetTextAsync(password);
            await this.ShowMessagePopup("Пароль скопирован!");
        }

        private async Task DoShowPassword(CredentialModel record)
        {
            var password = CryptographyUtils.Decrypt(userIdentity.GetUserKey(), record.Password);
            await this.ShowMessageWindow($"Пароль: {password}");
        }

        private async Task<ShowMessageWindow> ShowMessageWindow(string message, bool isDialog = true)
        {
            var dialog = new ShowMessageWindow
            {
                DataContext = new ShowMessageViewModel
                {
                    Message = message
                }
            };

            var mainWindow = ((App)Application.Current).MainWindow;
            if (isDialog)
            {
                await dialog.ShowDialog(mainWindow);
            }
            else
            {
                dialog.Show(mainWindow);
            }
            
            return dialog;
        }

        private async Task ShowMessagePopup(string message)
        {
            var dialog = await ShowMessageWindow(message, false);
            await Task.Delay(1000);
            dialog.Close();
        }
    }
}