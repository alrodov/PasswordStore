namespace PasswordStore.UI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using PasswordStore.Lib.Entities;

    public class CredentialModel
    {
        public long Id { get; set; }
        
        public int OrderNumber { get; set; }
        
        [Display(Name = "Сервис")]
        public string ServiceName { get; set; }
        
        [Display(Name = "Логин")]
        public string Login { get; set; }
        
        public string Password { get; set; }
        
        public string OpenPassword { get; set; }
        
        public DateTime CreateDate { get; set; } = DateTime.Now;
        
        public string Note { get; set; }
        
        public bool IsPhoneNumberAuth { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public ICollection<SecretQuestionModel> SecretQuestions { get; set; } = new List<SecretQuestionModel>();
    }
}