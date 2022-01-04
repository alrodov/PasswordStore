namespace PasswordStore.UI.ViewModels
{
    using System.Reactive;
    using ReactiveUI;

    public class ShowMessageViewModel: ViewModelBase
    {
        public ShowMessageViewModel(): base()
        {
        }
        
        public string Message { get; set; }
    }
}