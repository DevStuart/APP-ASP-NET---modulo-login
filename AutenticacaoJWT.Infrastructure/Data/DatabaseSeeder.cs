using AutenticacaoJWT.Domain.Entities;
using AutenticacaoJWT.Domain.Interfaces;
using AutenticacaoJWT.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutenticacaoJWT.Infra.Data.Data
{
    public class DatabaseSeeder
    {
        private readonly ContextApp _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(
            ContextApp context,
            IPasswordHasher passwordHasher,
            IConfiguration configuration,
            ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            var seed = _configuration.GetSection("LoginModule:SeedAdmin");
            if (!bool.TryParse(seed["Enabled"], out var seedEnabled) || !seedEnabled)
                return;

            var email = seed["Email"];
            var password = seed["Password"];
            var name = seed["Name"] ?? "Administrador";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return;

            var normalizedEmail = email.Trim().ToLower();
            var exists = await _context.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
            if (exists)
                return;

            var salt = _passwordHasher.GenerateSalt();
            var admin = new User
            {
                Name = name,
                Email = normalizedEmail,
                IsAdmin = true,
                Salt = salt,
                Password = _passwordHasher.HashPassword(password, salt)
            };

            _context.Users.Add(admin);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Usuário administrador seed criado: {Email}", normalizedEmail);
        }
    }
}
