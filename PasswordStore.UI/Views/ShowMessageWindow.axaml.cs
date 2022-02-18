using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PasswordStore.UI.Views
{
    using Avalonia.Input;
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

        private void MessageWindow_OnKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}