namespace PasswordStore.UI.Views
{
    using System;
    using System.Threading.Tasks;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Avalonia.ReactiveUI;
    using PasswordStore.UI.Models;
    using PasswordStore.UI.ViewModels;
    using ReactiveUI;

    public class PasswordGridView : ReactiveUserControl<PasswordGridViewModel>
    {
        public PasswordGridView()
        {
            InitializeComponent();

            this.WhenActivated(d => d(ViewModel!.AddCredential.RegisterHandler(ShowEditDialog)));
            this.WhenActivated(d => d(ViewModel!.EditCredential.RegisterHandler(ShowEditDialog)));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async Task ShowEditDialog(
            InteractionContext<CredentialFormViewModel, CredentialModel> interactionContext)
        {
            var dialog = new CredentialFormWindow
            {
                DataContext = interactionContext.Input
            };

            var result = await dialog.ShowDialog<CredentialModel>(((App)Application.Current).MainWindow);
            interactionContext.SetOutput(result);
        }
    }
}