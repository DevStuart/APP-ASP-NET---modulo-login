namespace AutenticacaoJWT.Domain.Models
{
    public class AuthTokens
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime RefreshExpiresAt { get; set; }
    }
}
