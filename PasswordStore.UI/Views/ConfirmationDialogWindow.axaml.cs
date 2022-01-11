namespace PasswordStore.UI.Views
{
    using System;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Avalonia.ReactiveUI;
    using PasswordStore.UI.ViewModels;
    using ReactiveUI;

    public class ConfirmationDialogWindow : ReactiveWindow<ConfirmationDialogViewModel>
    {
        public ConfirmationDialogWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.WhenActivated(d => d(ViewModel!.Confirm.Subscribe(result => Close(result))));
            this.WhenActivated(d => d(ViewModel!.Decline.Subscribe(result => Close(result))));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}