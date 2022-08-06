namespace PasswordStore.UI.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive;
    using System.Threading;
    using System.Threading.Tasks;
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
            this.Add = ReactiveCommand.Create(DoAdd);
            this.Remove = ReactiveCommand.Create<SecretQuestionModel>(DoRemove);
        }
        
        public ReactiveCommand<Unit, Unit> Add { get; private set; }
        
        public ReactiveCommand<SecretQuestionModel, Unit> Remove { get; private set; }

        public bool IsPhoneNumber
        {
            get => isPhoneNumber;
            set
            {
                this.Data.IsPhoneNumberAuth = value;
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
                this.phoneNumber = value?.PhoneNumber ?? string.Empty;
            }
        }

        public ObservableCollection<SecretQuestionModel> SecretQuestions => new (data.SecretQuestions);

        public ReactiveCommand<Unit, CredentialModel> ConfirmData { get; set; }

        private CredentialModel DoConfirmData()
        {
            return Data;
        }

        private void DoAdd()
        {
            this.Data.SecretQuestions.Add(new SecretQuestionModel());
            this.RaisePropertyChanged(nameof(SecretQuestions));
        }

        private void DoRemove(SecretQuestionModel record)
        {
            this.Data.SecretQuestions.Remove(record);
            this.RaisePropertyChanged(nameof(SecretQuestions));
        }
    }
}