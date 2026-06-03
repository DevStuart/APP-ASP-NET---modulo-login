using AutenticacaoJWT.Domain.Models;

namespace AutenticacaoJWT.Application.DTO
{
    public class AuthTokensDTO
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime RefreshExpiresAt { get; set; }

        public static AuthTokensDTO FromDomain(AuthTokens tokens) => new()
        {
            Token = tokens.Token,
            RefreshToken = tokens.RefreshToken,
            ExpiresAt = tokens.ExpiresAt,
            RefreshExpiresAt = tokens.RefreshExpiresAt
        };
    }
}
