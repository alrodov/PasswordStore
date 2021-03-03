namespace PasswordStore.Lib.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Сущность хранимого пароля
    /// </summary>
    public class Credential : BaseEntity
    {
        /// <summary>
        /// Идентификатор пользователя, владеющего паролем
        /// </summary>
        [Required]
        public long UserId { get; set; }
        
        /// <summary>
        /// Название сервиса, к которому относится пароль
        /// </summary>
        [Required]
        [Display(Name = "Сервис")]
        public string ServiceName { get; set; }
        
        /// <summary>
        /// Логин
        /// </summary>
        [Required]
        [Display(Name = "Логин")]
        public string Login { get; set; }
        
        /// <summary>
        /// Пароль
        /// </summary>
        [Required]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}