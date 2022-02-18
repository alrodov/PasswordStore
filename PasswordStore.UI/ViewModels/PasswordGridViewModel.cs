namespace PasswordStore.UI.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Avalonia;
    using Microsoft.Extensions.DependencyInjection;
    using PasswordStore.Lib.Crypto;
    using PasswordStore.Lib.Extension;
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
        private bool mainViewEnabled;
        private bool maskVisible;
        private string currentSortingProperty = string.Empty;
        private bool? ascSortOrder = null;
        private string filterValue = string.Empty;
        private bool clearFilterEnabled = false;
        
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

        public bool MainViewEnabled
        {
            get => mainViewEnabled;
            set => this.RaiseAndSetIfChanged(ref mainViewEnabled, value);
        }

        public bool MaskVisible
        {
            get => maskVisible;
            set => this.RaiseAndSetIfChanged(ref maskVisible, value);
        }

        public string FilterValue
        {
            get => filterValue;
            set
            {
                this.RaiseAndSetIfChanged(ref filterValue, value);
                ClearFilterEnabled = !string.IsNullOrEmpty(filterValue);
                FilterSet(value);
            }
        }

        public bool ClearFilterEnabled
        {
            get => clearFilterEnabled;
            set => this.RaiseAndSetIfChanged(ref clearFilterEnabled, value);
        }

        public ReactiveCommand<Unit, Unit> ClearFilter { get; private set; }
        
        public ReactiveCommand<Unit, Unit> LoadData { get; private set; }
        
        public ReactiveCommand<CredentialModel, Unit> ShowPassword { get; private set; }
        
        public ReactiveCommand<CredentialModel, Unit> CopyToClipboard { get; private set; }

        public ReactiveCommand<Unit, Unit> Add { get; private set; }
        
        public ReactiveCommand<CredentialModel, Unit> Edit { get; private set; }
        
        public ReactiveCommand<CredentialModel, Unit> Remove { get; private set; }
        
        public Interaction<CredentialFormViewModel, CredentialModel> AddCredential { get; set; }
        
        public Interaction<CredentialFormViewModel, CredentialModel> EditCredential { get; set; }
        
        public Interaction<ConfirmationDialogViewModel, bool?> ConfirmRemoveCredential { get; set; }
        
        private void InitCommands()
        {
            this.ClearFilter = ReactiveCommand.CreateFromTask(DoClearFilter);
            this.LoadData = ReactiveCommand.CreateFromTask(DoLoadData);
            this.ShowPassword = ReactiveCommand.CreateFromTask<CredentialModel>(DoShowPassword);
            this.CopyToClipboard = ReactiveCommand.CreateFromTask<CredentialModel>(DoCopyPasswordToClipboard);
            this.Add = ReactiveCommand.CreateFromTask(DoAdd);
            this.Edit = ReactiveCommand.CreateFromTask<CredentialModel>(DoEdit);
            this.Remove = ReactiveCommand.CreateFromTask<CredentialModel>(DoRemove);

            this.EditCredential = new Interaction<CredentialFormViewModel, CredentialModel>();
            this.AddCredential = new Interaction<CredentialFormViewModel, CredentialModel>();
            this.ConfirmRemoveCredential = new Interaction<ConfirmationDialogViewModel, bool?>();
        }

        private async Task DoClearFilter()
        {
            FilterValue = string.Empty;
            ClearFilterEnabled = false;
        }
        
        private async Task DoLoadData()
        {
            this.Credentials = new List<CredentialModel>();
            this.Mask();
            var cts = new CancellationTokenSource(App.DefaultTimeoutMilliseconds);
            var data = await Task.Run(async () => await credentialService.ListCredentialsAsync(this.filterValue, cts.Token), cts.Token);
            var counter = 1;
            this.Credentials = data.Select(cr => new CredentialModel
            {
                Id = cr.Id,
                OrderNumber = counter++,
                ServiceName = cr.ServiceName,
                Login = cr.Login,
                Password = cr.Password
            }).ToList();

            this.Unmask();
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
                this.Mask();
                try
                {
                    var cts = new CancellationTokenSource(App.DefaultTimeoutMilliseconds);
                    await Task.Run(async () => await this.credentialService.EditCredentialAsync(editedData.Id, editedData.ServiceName,
                        editedData.Login, editedData.OpenPassword, cts.Token), cts.Token);
                    await this.DoLoadData();
                }
                catch (Exception e)
                {
                    await this.ShowMessageWindow($"Произошла ошибка при сохранении данных: {e.Message}");
                }
                finally
                {
                    this.Unmask();
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
                    this.Mask();
                    var cts = new CancellationTokenSource(App.DefaultTimeoutMilliseconds);
                    await Task.Run(async () => await this.credentialService.AddCredentialAsync(addedData.ServiceName,
                        addedData.Login, addedData.OpenPassword, cts.Token), cts.Token);
                    await this.DoLoadData();
                }
                catch (Exception e)
                {
                    await this.ShowMessageWindow($"Произошла ошибка при сохранении данных: {e.Message}");
                }
                finally
                {
                    this.Unmask();
                }
            }
        }

        private async Task DoRemove(CredentialModel record)
        {
            var confirmation = await ConfirmRemoveCredential.Handle(new ConfirmationDialogViewModel
            {
                OperationName = "Удаление записи",
                ConfirmationText = $"Вы уверены, что хотите удалить пароль для сервиса {record.ServiceName}?"
            }).FirstOrDefaultAsync();

            if (confirmation == true)
            {
                try
                {
                    this.Mask();
                    var cts = new CancellationTokenSource(App.DefaultTimeoutMilliseconds);
                    await Task.Run(async () => await this.credentialService.RemoveCredentialAsync(record.Id, cts.Token), cts.Token);
                    await this.DoLoadData();
                }
                catch (Exception e)
                {
                    await this.ShowMessageWindow($"Произошла ошибка при удалении: {e.Message}");
                }
                finally
                {
                    this.Unmask();
                }
            }
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

        internal void EnumerateData(string? columnName = null)
        {
            var propertyName = typeof(CredentialModel).GetProperties()
                .Where(prop => prop.GetDisplayName()?.Equals(columnName, StringComparison.InvariantCultureIgnoreCase) == true)
                .Select(p => p.Name)
                .FirstOrDefault();
            Func<CredentialModel, object> propSelector = propertyName switch
            {
                nameof(CredentialModel.Login) => record => record.Login,
                nameof(CredentialModel.ServiceName) => record => record.ServiceName,
                _ => record => record.Id
            };

            IEnumerable<CredentialModel> enumeration;
            if (currentSortingProperty.Equals(propertyName))
            {
                enumeration = ascSortOrder == true
                    ? credentials.OrderBy(propSelector)
                    : credentials.OrderByDescending(propSelector);
                ascSortOrder = ascSortOrder != true;
            }
            else
            {
                enumeration = credentials.OrderBy(propSelector);
                currentSortingProperty = propertyName ?? string.Empty;
                ascSortOrder = true;
            }

            var counter = 1;
            foreach (var credential in enumeration)
            {
                credential.OrderNumber = counter++;
            }
        }

        private async Task FilterSet(string setValue)
        {
            if (!string.IsNullOrEmpty(setValue))
            {
                await Task.Delay(1000);
            }
            
            if (filterValue != setValue)
            {
                return;
            }

            await this.DoLoadData();
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

        private void Mask()
        {
            this.MainViewEnabled = false;
            this.MaskVisible = true;
        }

        private void Unmask()
        {
            this.MaskVisible = false;
            this.MainViewEnabled = true;
        }
    }
}