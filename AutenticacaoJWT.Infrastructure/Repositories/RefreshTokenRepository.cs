using AutenticacaoJWT.Domain.Entities;
using AutenticacaoJWT.Domain.Interfaces;
using AutenticacaoJWT.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace AutenticacaoJWT.Infra.Data.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ContextApp _context;

        public RefreshTokenRepository(ContextApp context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
        {
            return await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.TokenHash == tokenHash);
        }

        public async Task RevokeAllForUserAsync(int userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(r => r.UserId == userId && r.RevokedAt == null)
                .ToListAsync();

            foreach (var token in tokens)
                token.RevokedAt = DateTime.UtcNow;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await Task.CompletedTask;
        }
    }
}
