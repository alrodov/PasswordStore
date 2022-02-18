namespace PasswordStore.Lib.Entities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
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
        
        /// <summary>
        /// Примечание
        /// </summary>
        [Display(Name = "Примечание")]
        public string Note { get; set; }
        
        /// <summary>
        /// Дата создания
        /// </summary>
        [Display(Name = "Дата создания")]
        public DateTime CreateDate { get; set; }
        
        /// <summary>
        /// Вход по номеру телефона
        /// </summary>
        [Display(Name = "Вход по номеру телефона")]
        public bool IsPhoneNumberAuth { get; set; }
        
        /// <summary>
        /// Связанный номер телефона
        /// </summary>
        [Display(Name = "Связанный номер телефона")]
        public string PhoneNumber { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        
        public ICollection<SecretQuestion> SecretQuestions { get; set; }
    }
}