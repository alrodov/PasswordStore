namespace PasswordStore.UI.Views
{
    using System.Reactive.Linq;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;
    using Avalonia.ReactiveUI;
    using PasswordStore.UI.ViewModels;

    public class SignUpView : ReactiveUserControl<SignUpViewModel>
    {
        public SignUpView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void MainPanel_OnKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await this.ViewModel.Register.Execute();
            }
        }
    }
}