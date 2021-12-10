namespace PasswordStore.UI.Models
{
    public class SignUpResult
    {
        public bool Success { get; set; }
        
        public bool Cancelled { get; set; }
        
        public string? Message { get; set; }
    }
}