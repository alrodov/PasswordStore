namespace PasswordStore.CLI.Commands
{
    using CommandLine;
    /// <summary>
    /// Описатель команды смены ключа доступа
    /// </summary>
    [Verb("changemasterkey", HelpText = "Изменить ключ доступа активного пользователя")]
    public class ChangeMasterKey
    {
        /// <summary>
        /// Новое значение ключа доступа для пользователя
        /// </summary>
        [Option('k', "key", Required = true, HelpText = "Новое значение ключа доступа")]
        public string NewKey { get; set; }
    }
}