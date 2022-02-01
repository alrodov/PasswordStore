﻿namespace PasswordStore.UI.ViewModels
{
    using System;
    using System.Reactive;
    using System.Threading.Tasks;
    using Avalonia.Media;
    using Microsoft.Extensions.DependencyInjection;
    using PasswordStore.Lib.Interfaces;
    using PasswordStore.UI.Models;
    using ReactiveUI;

    public class LoginViewModel: ViewModelBase
    {
        private readonly IAuthService authService;
        
        private string? message;
        
        private IBrush messageColor = Brushes.White;

        public LoginViewModel(IServiceProvider serviceProvider)
        {
            this.authService = serviceProvider.GetRequiredService<IAuthService>();
            this.InitCommands();
        }
        
        public string Login { get; set; }
        
        public string Password { get; set; }

        public string? Message
        {
            get => message;
            set => this.RaiseAndSetIfChanged(ref message, value);
        }

        public IBrush MessageColor
        {
            get => messageColor;
            set => this.RaiseAndSetIfChanged(ref messageColor, value);
        }

        public ReactiveCommand<Unit, AuthResult> DoLogin { get; private set; }
        
        public ReactiveCommand<Unit, AuthResult> RequestSignUp { get; private set; }
        
        public void SetMessage(string? text, bool successful)
        {
            this.Message = text;
            this.MessageColor = successful ? Brushes.White : Brushes.Red;
        }
        
        private void InitCommands()
        {
            this.DoLogin = ReactiveCommand.CreateFromTask(DoLoginAsync);
            this.RequestSignUp = ReactiveCommand.Create(DoRequestSignUp);
        }

        private async Task<AuthResult> DoLoginAsync()
        {
            var result = new AuthResult();
            try
            {
                result.Success = await this.authService.LoginAsync(Login, Password);
                if (!result.Success)
                {
                    result.Message = "Попытка аутентификации была неуспешна";
                }
            }
            catch (Exception e)
            {
                result.Message = $"Произошла ошибка при входе пользователя:\n{e.Message}";
            }

            return result;
        }

        private AuthResult DoRequestSignUp()
        {
            return new AuthResult
            {
                SignUpRequested = true
            };
        }
    }
}