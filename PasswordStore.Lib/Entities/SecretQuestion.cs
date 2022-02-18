namespace PasswordStore.Lib.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Секретный вопрос
    /// </summary>
    public class SecretQuestion: BaseEntity
    {
        /// <summary>
        /// Запись учётных данных
        /// </summary>
        [Required]
        public long CredentialId { get; set; }
        
        /// <summary>
        /// Вопрос
        /// </summary>
        [Required]
        [Display(Name = "Вопрос")]
        public string Question { get; set; }
        
        /// <summary>
        /// Ответ
        /// </summary>
        [Required]
        [Display(Name = "Ответ")]
        public string Answer { get; set; }
        
        [ForeignKey(nameof(CredentialId))]
        public Credential Credential { get; set; }
    }
}