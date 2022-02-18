namespace PasswordStore.UI.Views
{
    using System.Threading.Tasks;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
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
            this.WhenActivated(d => d(ViewModel!.ConfirmRemoveCredential.RegisterHandler(ShowConfirmDialog)));
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

        private async Task ShowConfirmDialog(InteractionContext<ConfirmationDialogViewModel, bool?> interactionContext)
        {
            var dialog = new ConfirmationDialogWindow
            {
                DataContext = interactionContext.Input
            };

            var result = await dialog.ShowDialog<bool?>(((App)Application.Current).MainWindow);
            interactionContext.SetOutput(result);
        }

        private void PasswordsGrid_OnSorting(object? sender, DataGridColumnEventArgs e)
        {
            //this.ViewModel.EnumerateData(e.Column.Header?.ToString());
        }
    }
}