namespace PasswordStore.CLI.Commands
{
    using CommandLine;
    /// <summary>
    /// Описатель команды вывода списка паролей
    /// </summary>
    [Verb("list", HelpText = "Вывести сохраненные пароли")]
    public class ListCredentials
    {
        /// <summary>
        /// Значение фильтра по имени сервиса
        /// </summary>
        [Option('f', "filter", Required = false, HelpText = "Значение для фильтрации по сервисам (поиск по частичному совпадению без учёта регистра)")]
        public string Filter { get; set; }
    }
}