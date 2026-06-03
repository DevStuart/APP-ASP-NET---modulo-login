using AutenticacaoJWT.API.Models;
using AutenticacaoJWT.Application.DTO;
using AutenticacaoJWT.Application.Interfaces;
using AutenticacaoJWT.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace AutenticacaoJWT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("auth")]
    public class LoginController : ControllerBase
    {
        private const string InvalidCredentialsMessage = "Email ou senha inválidos.";
        private readonly IAuthenticate _authenticateService;
        private readonly IUserService _userService;

        public LoginController(IAuthenticate authenticateService, IUserService userService)
        {
            _authenticateService = authenticateService;
            _userService = userService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] CreateUserDTO createUserDTO)
        {
            if (await _authenticateService.UserExists(createUserDTO.Email))
                return BadRequest(new { message = "Email já cadastrado." });

            var user = await _userService.AddUser(createUserDTO);
            if (user == null)
                return BadRequest(new { message = "Erro ao cadastrar usuário." });

            var tokens = await _authenticateService.IssueTokensAsync(user.Id, user.Email, user.IsAdmin);
            return Ok(AuthResponse.FromTokens(tokens));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequestDTO loginRequest)
        {
            var authenticated = await _authenticateService.AuthenticateAsync(
                loginRequest.Email,
                loginRequest.Password);

            if (!authenticated)
                return Unauthorized(new { message = InvalidCredentialsMessage });

            var user = await _authenticateService.GetUserByEmail(loginRequest.Email);
            if (user == null)
                return Unauthorized(new { message = InvalidCredentialsMessage });

            var tokens = await _authenticateService.IssueTokensAsync(user.Id, user.Email, user.IsAdmin);
            return Ok(AuthResponse.FromTokens(tokens));
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshTokenRequestDTO request)
        {
            var tokens = await _authenticateService.RefreshTokensAsync(request.RefreshToken);
            if (tokens == null)
                return Unauthorized(new { message = "Refresh token inválido ou expirado." });

            return Ok(AuthResponse.FromTokens(tokens));
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout([FromBody] RefreshTokenRequestDTO? request)
        {
            if (request != null && !string.IsNullOrWhiteSpace(request.RefreshToken))
                await _authenticateService.RevokeRefreshTokenAsync(request.RefreshToken);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _authenticateService.RevokeAllRefreshTokensAsync(userId);

            return Ok(new { message = "Logout realizado com sucesso." });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserResponseDTO>> Me()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _userService.GetUserById(userId);

            if (user == null)
                return NotFound(new { message = "Usuário não encontrado." });

            return Ok(user);
        }
    }
}
