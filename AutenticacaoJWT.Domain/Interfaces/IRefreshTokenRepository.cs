using AutenticacaoJWT.Domain.Entities;

namespace AutenticacaoJWT.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
        Task AddAsync(RefreshToken refreshToken);
        Task UpdateAsync(RefreshToken refreshToken);
        Task RevokeAllForUserAsync(int userId);
        Task SaveChangesAsync();
    }
}
