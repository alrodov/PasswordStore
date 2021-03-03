namespace PasswordStore.CLI.Commands
{
    using CommandLine;
    /// <summary>
    /// Описатель команды поиска пароля
    /// </summary>
    [Verb("find", HelpText = "Найти пароль")]
    public class FindPassword
    {
        /// <summary>
        /// Имя сервиса, к которому относится пароль
        /// </summary>
        [Option('s', "service", Required = true, HelpText = "Название сервиса, пароль от которого нужно найти")]
        public string ServiceName { get; set; }
        
        /// <summary>
        /// Логин учетной записи, к которой относится пароль
        /// </summary>
        [Option('l', "login", Required = false, HelpText = "Логин пользователя в сервисе")]
        public string Login { get; set; }
    }
}