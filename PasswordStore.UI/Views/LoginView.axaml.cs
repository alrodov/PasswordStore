namespace PasswordStore.UI.Views
{
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;
    using Avalonia.ReactiveUI;
    using PasswordStore.UI.ViewModels;

    public class LoginView : ReactiveUserControl<LoginViewModel>
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void LoginView_OnKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await this.ViewModel.DoLogin.Execute();
            }
        }
    }
}