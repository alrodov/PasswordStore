namespace PasswordStore.UI.Models
{
    using PasswordStore.Lib.Entities;

    public class CredentialModel
    {
        public long Id { get; set; }
        
        public string ServiceName { get; set; }
        
        public string Login { get; set; }
        
        public string Password { get; set; }
    }
}