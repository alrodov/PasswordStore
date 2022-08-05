namespace PasswordStore.UI.ViewModels
{
    using System.Reactive;
    using PasswordStore.UI.Models;
    using ReactiveUI;

    public class CredentialFormViewModel: ViewModelBase
    {
        private CredentialModel data;
        private bool isPhoneNumber;
        private string phoneNumber;
        
        public CredentialFormViewModel(): base()
        {
            this.ConfirmData = ReactiveCommand.Create(DoConfirmData);
        }

        public bool IsPhoneNumber
        {
            get => isPhoneNumber;
            set
            {
                this.RaiseAndSetIfChanged(ref isPhoneNumber, value);
                if (!value)
                {
                    this.PhoneNumber = string.Empty;
                }
            }
        }

        public string PhoneNumber
        {
            get => phoneNumber;
            set
            {
                this.Data.PhoneNumber = value;
                this.RaiseAndSetIfChanged(ref phoneNumber, value);
            }
        }

        public string CreateDateText => $"Дата создания: {Data.CreateDate:dd.MM.yyyy}";

        public string DataOperationName { get; set; }
        
        public CredentialModel Data
        {
            get => data;
            set
            {
                this.data = value;
                this.isPhoneNumber = value?.IsPhoneNumberAuth ?? false;
                this.PhoneNumber = value?.PhoneNumber ?? string.Empty;
            }
        }
        
        public ReactiveCommand<Unit, CredentialModel> ConfirmData { get; set; }

        private CredentialModel DoConfirmData()
        {
            return Data;
        }
    }
}