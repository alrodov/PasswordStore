namespace PasswordStore.CLI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CommandLine;
    using ConsoleTableExt;
    using Microsoft.Extensions.DependencyInjection;
    using NLog;
    using PasswordStore.CLI.Commands;
    using PasswordStore.Core;
    using PasswordStore.Lib.Crypto;
    using PasswordStore.Lib.Entities;
    using PasswordStore.Lib.Interfaces;

    public class ApplicationLifecycle
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        private IUserService userService;

        private IAuthService authService;

        private ICredentialService credentialService;

        private IUserIdentity userIdentity;
        
        public ApplicationLifecycle(IServiceProvider serviceProvider)
        {
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.authService = serviceProvider.GetRequiredService<IAuthService>();
            this.credentialService = serviceProvider.GetRequiredService<ICredentialService>();
            this.userIdentity = serviceProvider.GetRequiredService<IUserIdentity>();
        }

        public void Run()
        {
            this.SignIn();
            this.RunMainCycle();
        }
        
        /// <summary>
        /// Основной цикл работы приложения
        /// </summary>
        private void RunMainCycle()
        {
            var exit = false;
            while (!exit)
            {
                var args = Console.ReadLine().Split(' ');
                var message = string.Empty;
                try
                {
                    using (var scope = Application.ServiceProvider.CreateScope())
                    {
                        message = Parser.Default
                            .ParseArguments<ListCredentials, FindPassword, AddCredential, UpdatePassword,
                                DeleteCredential,
                                ChangeMasterKey, Exit>(args)
                            .MapResult<ListCredentials, FindPassword, AddCredential, UpdatePassword, DeleteCredential,
                                ChangeMasterKey, Exit, string>(
                                List,
                                Find,
                                Add,
                                Update,
                                Delete,
                                ChangeMasterKey,
                                _ =>
                                {
                                    Exit();
                                    exit = true;
                                    return null;
                                },
                                errors => string.Join(Environment.NewLine, errors));
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e, e.Message);
                    message = e.Message;
                }

                if (!string.IsNullOrEmpty(message))
                {
                    Console.WriteLine(message);
                }
            }
        }

        /// <summary>
        /// Выполняет вход пользователя в приложение
        /// </summary>
        private void SignIn()
        {
            Console.WriteLine("Необходимо выполнить вход в приложение");
            Console.Write("Логин: ");
            var login = Console.ReadLine();
            var user = this.userService.FindUser(login);
            if (user == null)
            {
                Console.WriteLine($"Пользователь с логином {login} не найден");
                SignUp(login);
            }
            else
            {
                Console.Write($"Ключ доступа пользователя {login}: ");
                var key = this.ReadPassword();
                this.authService.Login(login, key);
            }
        }

        /// <summary>
        /// Регистрирует нового пользователя
        /// </summary>
        /// <param name="login">Логин нового пользователя</param>
        private void SignUp(string login)
        {
            Console.WriteLine("Выполняется регистрация нового пользователя");
            Console.Write($"Ключ доступа для пользователя {login}: ");
            var key = Console.ReadLine();
            
            this.userService.Register(login, key);
            this.authService.Login(login, key);
        }

        /// <summary>
        /// Обработчик команды вывода списка паролей
        /// </summary>
        private string List(ListCredentials command)
        {
            IList<Credential> data;
            if (string.IsNullOrEmpty(command.Filter))
            {
                data = this.credentialService.ListAllCredentials();
            }
            else
            {
                data = this.credentialService.FindByName(command.Filter, false);
            }
            
            PrintCredentials(data);
            return null;
        }

        /// <summary>
        /// Обработчик команды поиска пароля
        /// </summary>
        private string Find(FindPassword command)
        {
            var data = this.credentialService.FindByName(command.ServiceName, true);
            if (!string.IsNullOrEmpty(command.Login))
            {
                var filterValue = command.Login.ToLowerInvariant();
                data = data.Where(x => x.Login == filterValue).ToList();
            }
            
            PrintCredentials(data);
            return null;
        }
        
        /// <summary>
        /// Обработчик команды добавления пароля
        /// </summary>
        private string Add(AddCredential command)
        {
            this.credentialService.AddCredential(command.ServiceName, command.Login, command.Password);
            return "Данные сохранены";
        }
        
        /// <summary>
        /// Обработчик команды обновления пароля
        /// </summary>
        private string Update(UpdatePassword command)
        {
            this.credentialService.ChangePassword(command.ServiceName, command.Login, command.Password);
            return "Пароль изменён";
        }
        
        /// <summary>
        /// Обработчик команды удаления пароля
        /// </summary>
        private string Delete(DeleteCredential command)
        {
            this.credentialService.RemoveCredential(command.ServiceName, command.Login);
            return "Данные удалены";
        }
        
        /// <summary>
        /// Обработчик команды смены ключа доступа
        /// </summary>
        private string ChangeMasterKey(ChangeMasterKey command)
        {
            this.credentialService.ChangeMasterKey(command.NewKey);
            return "Данные изменены";
        }

        /// <summary>
        /// Обработчик команды выхода из приложения
        /// </summary>
        private void Exit()
        {
            this.authService.Logout();
        }

        private string ReadPassword()
        {
            var result = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(true);
                key = keyInfo.Key;
                if (key == ConsoleKey.Backspace && result.Length > 0)
                {
                    Console.Write("\b \b");
                    result = result[..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    result += keyInfo.KeyChar;
                }
            }
            while (key != ConsoleKey.Enter);

            return result;
        }

        /// <summary>
        /// Выводит список паролей
        /// </summary>
        /// <param name="data">Данные для вывода</param>
        private void PrintCredentials(IEnumerable<Credential> data)
        {
            var key = this.userIdentity.GetUserKey();
            List<object> GetCredentialPrintData(int number, Credential item)
            {
                var list = new List<object>();
                list.Add(number);
                list.Add(item.ServiceName);
                list.Add(item.Login);
                var openPassword = CryptographyUtils.Decrypt(key, item.Password);
                list.Add(openPassword);

                return list;
            }

            var i = 0;
            List<List<object>> printData = new List<List<object>>();
            foreach (var item in data.OrderBy(x => x.ServiceName).ThenBy(x => x.Login))
            {
                printData.Add(GetCredentialPrintData(++i, item));
            }

            ConsoleTableBuilder.From(printData).WithColumn("№", "Сервис", "Логин", "Пароль").ExportAndWriteLine();
        }
    }
}