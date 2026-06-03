namespace AutenticacaoJWT.Application.DTO
{
    public class UpdateUserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public string? Password { get; set; }
    }
}
