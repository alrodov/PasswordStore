using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PasswordStore.UI.Views
{
    using Avalonia.Interactivity;

    public class ShowMessageWindow : Window
    {
        public ShowMessageWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OkButton_OnClick(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}