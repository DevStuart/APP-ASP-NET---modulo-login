namespace AutenticacaoJWT.Domain.Interfaces
{
    public interface IPasswordHasher
    {
        byte[] GenerateSalt();
        string HashPassword(string password, byte[] salt);
        bool VerifyPassword(string password, byte[] salt, string hashedPassword);
    }
}
