namespace PasswordStore.CLI.Commands
{
    using CommandLine;

    /// <summary>
    /// Описатель команды удаления пароля
    /// </summary>
    [Verb("delete", HelpText = "Удалить пароль")]
    public class DeleteCredential
    {
        /// <summary>
        /// Имя сервиса, к которому относится пароль
        /// </summary>
        [Option('s', "service", Required = true, HelpText = "Название сервиса, которому принадлежит пароль")]
        public string ServiceName { get; set; }
        
        /// <summary>
        /// Логин учетной записи, к которой относится пароль
        /// </summary>
        [Option('l', "login", Required = true, HelpText = "Логин пользователя в сервисе")]
        public string Login { get; set; }
    }
}