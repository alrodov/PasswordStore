namespace PasswordStore.Lib.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Пользователь менеджера паролей
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// Логин
        /// </summary>
        [Required]
        [Display(Name = "Логин")]
        public string Login { get; set; }
        
        /// <summary>
        /// Ключ доступа к менеджеру
        /// </summary>
        [Required]
        [Display(Name = "Ключ доступа")]
        public string MasterKey { get; set; }
        
        public ICollection<Credential> Credentials { get; set; }
    }
}