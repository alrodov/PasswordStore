namespace PasswordStore.CLI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CommandLine;
    using Microsoft.Extensions.DependencyInjection;
    using PasswordStore.AppStarter;
    using PasswordStore.CLI.Commands;
    using PasswordStore.Core;
    using PasswordStore.Lib.Crypto;
    using PasswordStore.Lib.Entities;
    using PasswordStore.Lib.Interfaces;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                AppStarter.StartApp();

                SignIn();

                RunApplicationCycle();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// Основной цикл работы приложения
        /// </summary>
        static void RunApplicationCycle()
        {
            var exit = false;
            while (!exit)
            {
                var args = Console.ReadLine().Split(' ');
                var message = Parser.Default
                    .ParseArguments<ListCredentials, FindPassword, AddCredential, UpdatePassword, DeleteCredential,
                        ChangeMasterKey, Exit>(args)
                    .MapResult<ListCredentials, FindPassword, AddCredential, UpdatePassword, DeleteCredential,
                        ChangeMasterKey, Exit, string>(
                        List,
                        Find,
                        Add,
                        Update,
                        Delete,
                        ChangeMasterKey,
                        _ => {
                            Exit();
                            exit = true;
                            return null;
                        },
                        errors => string.Join(Environment.NewLine, errors));

                if (!string.IsNullOrEmpty(message))
                {
                    Console.WriteLine(message);
                }
            }
        }

        /// <summary>
        /// Выполняет вход пользователя в приложение
        /// </summary>
        static void SignIn()
        {
            // TODO правильное внедрение зависимостей в SignIn и SignUp
            var serviceProvider = Application.ServiceProvider;
            Console.WriteLine("Необходимо выполнить вход в приложение");
            Console.Write("Логин: ");
            var login = Console.ReadLine();
            var user = serviceProvider.GetService<IUserService>().FindUser(login);
            if (user == null)
            {
                Console.WriteLine($"Пользователь с логином {login} не найден");
                SignUp(login);
            }
            else
            {
                Console.Write($"Ключ доступа пользователя {login}: ");
                var key = Console.ReadLine();
                var authService = serviceProvider.GetService<IAuthService>();
                authService.Login(login, key);
            }
        }

        /// <summary>
        /// Регистрирует нового пользователя
        /// </summary>
        /// <param name="login">Логин нового пользователя</param>
        static void SignUp(string login)
        {
            Console.WriteLine("Выполняется регистрация нового пользователя");
            Console.Write($"Ключ доступа для пользователя {login}: ");
            var key = Console.ReadLine();

            var serviceProvider = Application.ServiceProvider;
            serviceProvider.GetService<IUserService>().Register(login, key);
            serviceProvider.GetService<IAuthService>().Login(login, key);
        }

        /// <summary>
        /// Обработчик команды вывода списка паролей
        /// </summary>
        static string List(ListCredentials command)
        {
            IList<Credential> data;
            var service = Application.ServiceProvider.GetService<ICredentialService>();
            if (string.IsNullOrEmpty(command.Filter))
            {
                data = service.ListAllCredentials();
            }
            else
            {
                data = service.FindByName(command.Filter, false);
            }
            
            PrintCredentials(data);
            return null;
        }

        /// <summary>
        /// Обработчик команды поиска пароля
        /// </summary>
        static string Find(FindPassword command)
        {
            var service = Application.ServiceProvider.GetService<ICredentialService>();
            var data = service.FindByName(command.ServiceName, true);
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
        static string Add(AddCredential command)
        {
            var service = Application.ServiceProvider.GetService<ICredentialService>();
            service.SetCredential(command.ServiceName, command.Login, command.Password);
            return "Данные сохранены";
        }
        
        /// <summary>
        /// Обработчик команды обновления пароля
        /// </summary>
        static string Update(UpdatePassword command)
        {
            var service = Application.ServiceProvider.GetService<ICredentialService>();
            service.SetCredential(command.ServiceName, command.Login, command.Password);
            return "Пароль изменён";
        }
        
        /// <summary>
        /// Обработчик команды удаления пароля
        /// </summary>
        static string Delete(DeleteCredential command)
        {
            var service = Application.ServiceProvider.GetService<ICredentialService>();
            service.RemoveCredential(command.ServiceName, command.Login);
            return "Данные удалены";
        }
        
        /// <summary>
        /// Обработчик команды смены ключа доступа
        /// </summary>
        static string ChangeMasterKey(ChangeMasterKey command)
        {
            var service = Application.ServiceProvider.GetService<ICredentialService>();
            service.ChangeMasterKey(command.NewKey);
            return "Данные изменены";
        }

        /// <summary>
        /// Обработчик команды выхода из приложения
        /// </summary>
        static void Exit()
        {
            var service = Application.ServiceProvider.GetService<IAuthService>();
            service.Logout();
        }

        /// <summary>
        /// Выводит список паролей
        /// </summary>
        /// <param name="data">Данные для вывода</param>
        static void PrintCredentials(IEnumerable<Credential> data)
        {
            var key = Application.ServiceProvider.GetService<IUserIdentity>().GetUserKey();
            var template = "{0}\t\t{1}\t\t{2}\t\t{3}";
            Console.WriteLine(template, "№", "Сервис", "Логин", "Пароль");
            var i = 0;
            foreach (var item in data.OrderBy(x => x.ServiceName).ThenBy(x => x.Login))
            {
                var openPassword = CryptographyUtils.Decrypt(key, item.Password);
                Console.WriteLine(template, ++i, item.ServiceName, item.Login, openPassword);
            }
        }
    }
}