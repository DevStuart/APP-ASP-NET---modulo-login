using AutenticacaoJWT.Domain.Entities;
using AutenticacaoJWT.Domain.Interfaces;
using AutenticacaoJWT.Domain.Models;
using AutenticacaoJWT.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AutenticacaoJWT.Infra.Data.Identity
{
    public class AuthenticateService : IAuthenticate
    {
        private readonly ContextApp _context;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthenticateService(
            ContextApp context,
            IConfiguration configuration,
            IPasswordHasher passwordHasher,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<bool> AuthenticateAsync(string email, string passwordInput)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
                return false;

            return _passwordHasher.VerifyPassword(passwordInput, user.Salt, user.Password);
        }

        public async Task<AuthTokens> IssueTokensAsync(int userId, string email, bool isAdmin)
        {
            var accessToken = GenerateAccessToken(userId, email, isAdmin);
            var (refreshToken, refreshExpires) = await CreateRefreshTokenAsync(userId);

            return new AuthTokens
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(GetAccessTokenMinutes()),
                RefreshExpiresAt = refreshExpires
            };
        }

        public async Task<AuthTokens?> RefreshTokensAsync(string refreshToken)
        {
            var tokenHash = HashRefreshToken(refreshToken);
            var stored = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

            if (stored == null || stored.RevokedAt != null || stored.ExpiresAt <= DateTime.UtcNow)
                return null;

            var user = stored.User ?? await _context.Users.FindAsync(stored.UserId);
            if (user == null)
                return null;

            stored.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(stored);
            await _refreshTokenRepository.SaveChangesAsync();

            return await IssueTokensAsync(user.Id, user.Email, user.IsAdmin);
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            var tokenHash = HashRefreshToken(refreshToken);
            var stored = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

            if (stored == null || stored.RevokedAt != null)
                return false;

            stored.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(stored);
            await _refreshTokenRepository.SaveChangesAsync();
            return true;
        }

        public async Task RevokeAllRefreshTokensAsync(int userId)
        {
            await _refreshTokenRepository.RevokeAllForUserAsync(userId);
            await _refreshTokenRepository.SaveChangesAsync();
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        private string GenerateAccessToken(int id, string email, bool isAdmin)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, id.ToString()),
                new("id", id.ToString()),
                new(ClaimTypes.Email, email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (isAdmin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            var privateKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["jwt:secretKey"]!));

            var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["jwt:issuer"],
                audience: _configuration["jwt:audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(GetAccessTokenMinutes()),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<(string Token, DateTime ExpiresAt)> CreateRefreshTokenAsync(int userId)
        {
            var plainToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var expiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenDays());

            var entity = new RefreshToken
            {
                UserId = userId,
                TokenHash = HashRefreshToken(plainToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            };

            await _refreshTokenRepository.AddAsync(entity);
            await _refreshTokenRepository.SaveChangesAsync();

            return (plainToken, expiresAt);
        }

        private static string HashRefreshToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

        private int GetAccessTokenMinutes() =>
            int.TryParse(_configuration["jwt:accessTokenMinutes"], out var minutes) ? minutes : 30;

        private int GetRefreshTokenDays() =>
            int.TryParse(_configuration["jwt:refreshTokenDays"], out var days) ? days : 7;
    }
}
