using AutenticacaoJWT.Application.Interfaces;
using AutenticacaoJWT.Application.Mappings;
using AutenticacaoJWT.Application.Services;
using AutenticacaoJWT.Application.Validators;
using AutenticacaoJWT.Domain.Interfaces;
using AutenticacaoJWT.Infra.Data.Context;
using AutenticacaoJWT.Infra.Data.Data;
using AutenticacaoJWT.Infra.Data.Identity;
using AutenticacaoJWT.Infra.Data.Repositories;
using AutenticacaoJWT.Infra.Data.Security;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AutenticacaoJWT.Infra.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddLoginModule(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddInfrastructure(configuration);
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ContextApp>(options =>
                options.UseNpgsql(configuration.GetConnectionString("AutenticacaoJWTDB")));

            services.AddAutoMapper(typeof(DomainToDTOMappingProfile));
            services.AddValidatorsFromAssemblyContaining<CreateUserDTOValidator>();

            var secretKey = configuration["jwt:secretKey"];
            if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
                throw new InvalidOperationException(
                    "Configure jwt:secretKey com pelo menos 32 caracteres (User Secrets ou variáveis de ambiente).");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["jwt:issuer"],
                    ValidAudience = configuration["jwt:audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthenticate, AuthenticateService>();
            services.AddScoped<DatabaseSeeder>();

            return services;
        }

        public static IServiceCollection AddLoginModuleCors(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var origins = configuration.GetSection("LoginModule:CorsOrigins").Get<string[]>()
                ?? Array.Empty<string>();

            services.AddCors(options =>
            {
                options.AddPolicy("LoginModule", policy =>
                {
                    if (origins.Length > 0)
                    {
                        policy.WithOrigins(origins)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                });
            });

            return services;
        }
    }
}
