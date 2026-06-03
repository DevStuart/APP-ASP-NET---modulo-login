using AutenticacaoJWT.Domain.Models;

namespace AutenticacaoJWT.API.Models
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime RefreshExpiresAt { get; set; }

        public static AuthResponse FromTokens(AuthTokens tokens) => new()
        {
            Token = tokens.Token,
            RefreshToken = tokens.RefreshToken,
            ExpiresAt = tokens.ExpiresAt,
            RefreshExpiresAt = tokens.RefreshExpiresAt
        };
    }
}
