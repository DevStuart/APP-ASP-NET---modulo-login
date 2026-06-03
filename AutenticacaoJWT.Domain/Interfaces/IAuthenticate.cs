using AutenticacaoJWT.Domain.Entities;
using AutenticacaoJWT.Domain.Models;

namespace AutenticacaoJWT.Domain.Interfaces
{
    public interface IAuthenticate
    {
        Task<bool> AuthenticateAsync(string email, string password);
        Task<bool> UserExists(string email);
        Task<User?> GetUserByEmail(string email);
        Task<AuthTokens> IssueTokensAsync(int userId, string email, bool isAdmin);
        Task<AuthTokens?> RefreshTokensAsync(string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);
        Task RevokeAllRefreshTokensAsync(int userId);
    }
}
