namespace PasswordStore.UI.Models
{
    public class AuthResult
    {
        public bool Success { get; set; }
        
        public string? Message { get; set; }
        
        public bool SignUpRequested { get; set; }
    }
}