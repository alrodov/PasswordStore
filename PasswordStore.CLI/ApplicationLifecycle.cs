namespace PasswordStore.CLI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
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
        private const int DefaultCancellationTimeoutMinutes = 1;
        
        private const string SuccessfulAuthMessage = "Пользователь успешно аутентифицирован";
        
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

        public async Task RunAsync()
        {
            await this.SignIn();
            await this.RunMainCycle();
        }
        
        /// <summary>
        /// Основной цикл работы приложения
        /// </summary>
        private async Task RunMainCycle()
        {
            var exit = false;
            while (!exit)
            {
                Console.Write(">");
                var args = Console.ReadLine()!.Split(' ');
                try
                {
                    using (var scope = Application.ServiceProvider.CreateScope())
                    {
                        var parseResult = Parser.Default
                            .ParseArguments<ListCredentials, FindPassword, AddCredential, UpdatePassword,
                                DeleteCredential,
                                ChangeMasterKey, Exit>(args);
                        await parseResult.WithParsedAsync<ListCredentials>(List);
                        await parseResult.WithParsedAsync<FindPassword>(Find);
                        await parseResult.WithParsedAsync<AddCredential>(Add);
                        await parseResult.WithParsedAsync<UpdatePassword>(Update);
                        await parseResult.WithParsedAsync<DeleteCredential>(Delete);
                        await parseResult.WithParsedAsync<ChangeMasterKey>(ChangeMasterKey);
                        parseResult.WithParsed<Exit>(_ =>
                        {
                            Exit();
                            exit = true;
                        });

                        parseResult.WithNotParsed(errors => string.Join(Environment.NewLine, errors));
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e, e.Message);
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Выполняет вход пользователя в приложение
        /// </summary>
        private async Task SignIn()
        {
            Console.WriteLine("Необходимо выполнить вход в приложение");
            Console.Write("Логин: ");
            var login = Console.ReadLine();
            var user = await this.userService.FindUserAsync(login);
            if (user == null)
            {
                Console.WriteLine($"Пользователь с логином {login} не найден");
                await this.SignUp(login);
            }
            else
            {
                Console.Write($"Ключ доступа пользователя {login}: ");
                var key = this.ReadPassword();
                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(DefaultCancellationTimeoutMinutes));
                if (await this.authService.LoginAsync(login, key, cts.Token))
                {
                    Console.WriteLine(SuccessfulAuthMessage);
                }
            }
        }

        /// <summary>
        /// Регистрирует нового пользователя
        /// </summary>
        /// <param name="login">Логин нового пользователя</param>
        private async Task SignUp(string login)
        {
            Console.WriteLine("Выполняется регистрация нового пользователя");
            Console.Write($"Ключ доступа для пользователя {login}: ");
            var key = Console.ReadLine();
            Console.Write($"Подтвердите ключ доступа: ");
            var keyConfirm = Console.ReadLine();
            if (key != keyConfirm)
            {
                Console.WriteLine("Ключи доступа не совпадают");
                return;
            }
            
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
            await this.userService.RegisterAsync(login, key, cts.Token);
            if (await this.authService.LoginAsync(login, key, cts.Token))
            {
                Console.WriteLine(SuccessfulAuthMessage);
            }
        }

        /// <summary>
        /// Обработчик команды вывода списка паролей
        /// </summary>
        private async Task List(ListCredentials command)
        {
            IList<Credential> data;
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(DefaultCancellationTimeoutMinutes));
            if (string.IsNullOrEmpty(command.Filter))
            {
                data = await this.credentialService.ListAllCredentialsAsync(cts.Token);
            }
            else
            {
                data = await this.credentialService.FindByNameAsync(command.Filter, false, cts.Token);
            }
            
            PrintCredentials(data);
        }

        /// <summary>
        /// Обработчик команды поиска пароля
        /// </summary>
        private async Task Find(FindPassword command)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(DefaultCancellationTimeoutMinutes));
            var data = await this.credentialService.FindByNameAsync(command.ServiceName, true, cts.Token);
            if (!string.IsNullOrEmpty(command.Login))
            {
                var filterValue = command.Login.ToLowerInvariant();
                data = data.Where(x => x.Login == filterValue).ToList();
            }
            
            PrintCredentials(data);
        }
        
        /// <summary>
        /// Обработчик команды добавления пароля
        /// </summary>
        private async Task Add(AddCredential command)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(DefaultCancellationTimeoutMinutes));
            await this.credentialService.AddCredentialAsync(command.ServiceName, command.Login, command.Password, cts.Token);
            Console.WriteLine("Данные сохранены");
        }
        
        /// <summary>
        /// Обработчик команды обновления пароля
        /// </summary>
        private async Task Update(UpdatePassword command)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(DefaultCancellationTimeoutMinutes));
            await this.credentialService.ChangePasswordAsync(command.ServiceName, command.Login, command.Password,cts.Token);
            Console.WriteLine("Пароль изменён");
        }
        
        /// <summary>
        /// Обработчик команды удаления пароля
        /// </summary>
        private async Task Delete(DeleteCredential command)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(DefaultCancellationTimeoutMinutes));
            await this.credentialService.RemoveCredentialAsync(command.ServiceName, command.Login, cts.Token);
            Console.WriteLine("Данные удалены");
        }
        
        /// <summary>
        /// Обработчик команды смены ключа доступа
        /// </summary>
        private async Task ChangeMasterKey(ChangeMasterKey command)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));
            await this.credentialService.ChangeMasterKeyAsync(command.NewKey, cts.Token);
            Console.WriteLine("Данные изменены");
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

            Console.WriteLine();

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