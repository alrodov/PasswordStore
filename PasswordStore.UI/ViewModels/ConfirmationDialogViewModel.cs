namespace PasswordStore.UI.ViewModels
{
    using System.Reactive;
    using ReactiveUI;

    public class ConfirmationDialogViewModel: ViewModelBase
    {
        public ConfirmationDialogViewModel() : base()
        {
            this.InitCommands();
        }
        
        public string OperationName { get; set; }
        
        public string ConfirmationText { get; set; }

        public ReactiveCommand<Unit, bool> Confirm { get; set; }
        
        public ReactiveCommand<Unit, bool> Decline { get; set; }

        private void InitCommands()
        {
            this.Confirm = ReactiveCommand.Create(() => true);
            this.Decline = ReactiveCommand.Create(() => false);
        }
    }
}