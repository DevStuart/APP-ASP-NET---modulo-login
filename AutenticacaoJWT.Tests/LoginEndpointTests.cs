using AutenticacaoJWT.Infra.Data.Context;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace AutenticacaoJWT.Tests
{
    public class LoginEndpointTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public LoginEndpointTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ContextApp>();
            db.Database.EnsureCreated();
        }

        [Fact]
        public async Task Register_And_Login_ShouldReturnTokens()
        {
            var email = $"user_{Guid.NewGuid():N}@test.com";
            var register = new
            {
                name = "Teste",
                email,
                password = "Senha@123"
            };

            var registerResponse = await _client.PostAsJsonAsync("/api/Login/register", register);
            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            var loginResponse = await _client.PostAsJsonAsync("/api/Login/login", new { email, password = "Senha@123" });
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var tokens = await loginResponse.Content.ReadFromJsonAsync<AuthResponseTest>();
            Assert.False(string.IsNullOrWhiteSpace(tokens?.Token));
            Assert.False(string.IsNullOrWhiteSpace(tokens?.RefreshToken));
        }

        private class AuthResponseTest
        {
            public string Token { get; set; } = string.Empty;
            public string RefreshToken { get; set; } = string.Empty;
        }
    }
}
