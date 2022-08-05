namespace PasswordStore.UI.Views
{
    using System;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Avalonia.Interactivity;
    using Avalonia.ReactiveUI;
    using PasswordStore.UI.Models;
    using PasswordStore.UI.ViewModels;
    using ReactiveUI;

    public class CredentialFormWindow : ReactiveWindow<CredentialFormViewModel>
    {
        public CredentialFormWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            this.WhenActivated(d => d(ViewModel!.ConfirmData.Subscribe(Close)));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void CancelEditClick(object? sender, RoutedEventArgs e)
        {
            this.Close(null);
        }

        private void IsPhoneNumberBoxOnChecked(object? sender, RoutedEventArgs e)
        {
            this.ViewModel.IsPhoneNumber = true;
        }

        private void IsPhoneNumberBoxOnUnchecked(object? sender, RoutedEventArgs e)
        {
            this.ViewModel.IsPhoneNumber = false;
        }
    }
}