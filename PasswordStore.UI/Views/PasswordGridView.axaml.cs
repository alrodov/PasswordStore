﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PasswordStore.UI.Views
{
    using System;
    using System.Threading.Tasks;
    using Avalonia.Interactivity;

    public class PasswordGridView : UserControl
    {
        public PasswordGridView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}