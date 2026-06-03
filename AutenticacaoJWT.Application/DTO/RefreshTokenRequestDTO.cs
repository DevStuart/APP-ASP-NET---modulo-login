using System.ComponentModel.DataAnnotations;

namespace AutenticacaoJWT.Application.DTO
{
    public class RefreshTokenRequestDTO
    {
        [Required(ErrorMessage = "Refresh token é obrigatório.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
