namespace PasswordStore.UI.ViewModels
{
    using System;
    using System.Reactive;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using PasswordStore.Lib.Interfaces;
    using PasswordStore.UI.Models;
    using ReactiveUI;

    public class SignUpViewModel: ViewModelBase
    {
        private readonly IUserService userService;
        private string message;

        public SignUpViewModel(IServiceProvider serviceProvider)
        {
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            
            this.InitCommands();
        }
        
        public string Login { get; set; }
        
        public string Password { get; set; }
        
        public string PasswordConfirmation { get; set; }

        public string Message
        {
            get => message;
            set => this.RaiseAndSetIfChanged(ref message, value);
        }
        
        public ReactiveCommand<Unit, SignUpResult> Register { get; private set; }
        
        public ReactiveCommand<Unit, SignUpResult> Cancel { get; private set; }

        private void InitCommands()
        {
            this.Register = ReactiveCommand.CreateFromTask(DoRegisterAsync);
            this.Cancel = ReactiveCommand.Create(() => new SignUpResult
            {
                Success = false,
                Cancelled = true
            });
        }

        private async Task<SignUpResult> DoRegisterAsync()
        {
            var result = new SignUpResult { Cancelled = false, Success = false };
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                Message = "Не задан логин или пароль пользователя";
                return result;
            }
            
            if (this.Password != PasswordConfirmation)
            {
                Message = "Введённые пароли не совпадают";
                return result;
            }

            try
            {
                await this.userService.RegisterAsync(Login, Password);
                result.Success = true;
                result.Message = "Пользователь успешно зарегистрирован";
            }
            catch (Exception e)
            {
                Message = $"Произошла ошибка при регистрации пользователя:\n{e.Message}";
            }

            return result;
        }
    }
}