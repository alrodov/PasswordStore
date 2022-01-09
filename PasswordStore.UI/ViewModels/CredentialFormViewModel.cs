namespace PasswordStore.UI.ViewModels
{
    using System.Reactive;
    using PasswordStore.UI.Models;
    using ReactiveUI;

    public class CredentialFormViewModel: ViewModelBase
    {
        public CredentialFormViewModel(): base()
        {
            this.ConfirmData = ReactiveCommand.Create(DoConfirmData);
        }

        public string DataOperationName { get; set; }
        
        public CredentialModel Data { get; set; }
        
        public ReactiveCommand<Unit, CredentialModel> ConfirmData { get; set; }

        private CredentialModel DoConfirmData()
        {
            return Data;
        }
    }
}