namespace PasswordStore.CLI.Commands
{
    using CommandLine;

    /// <summary>
    /// Описатель команды добавления пароля
    /// </summary>
    [Verb("add", HelpText = "Сохранить новый пароль")]
    public class AddCredential
    {
        /// <summary>
        /// Имя сервиса, к которому относится пароль
        /// </summary>
        [Option('s', "service", Required = true, HelpText = "Название сервиса, для которого сохраняется пароль")]
        public string ServiceName { get; set; }
        
        /// <summary>
        /// Логин учетной записи, к которой относится пароль
        /// </summary>
        [Option('l', "login", Required = true, HelpText = "Логин пользователя в сервисе")]
        public string Login { get; set; }
        
        /// <summary>
        /// Пароль
        /// </summary>
        [Option('p', "password", Required = true, HelpText = "Пароль")]
        public string Password { get; set; }
    }
}