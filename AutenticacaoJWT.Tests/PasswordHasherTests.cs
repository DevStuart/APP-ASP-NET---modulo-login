using AutenticacaoJWT.Infra.Data.Security;
using Xunit;

namespace AutenticacaoJWT.Tests
{
    public class PasswordHasherTests
    {
        [Fact]
        public void HashAndVerify_ShouldMatch()
        {
            var hasher = new PasswordHasher();
            var salt = hasher.GenerateSalt();
            var hash = hasher.HashPassword("Senha@123", salt);

            Assert.True(hasher.VerifyPassword("Senha@123", salt, hash));
            Assert.False(hasher.VerifyPassword("SenhaErrada", salt, hash));
        }
    }
}
