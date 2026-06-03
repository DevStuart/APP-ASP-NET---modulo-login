using AutenticacaoJWT.API.Extensions;
using AutenticacaoJWT.API.Models;
using AutenticacaoJWT.Application.DTO;
using AutenticacaoJWT.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoJWT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("Users")]
        public async Task<ActionResult> Get([FromQuery] PaginationParams paginationParams)
        {
            var users = await _userService.GetAllUsers(paginationParams.PageNumber, paginationParams.PageSize);

            if (users == null || users.Count == 0)
                return NotFound(new { message = "Usuários não encontrados." });

            Response.AddPaginationHeader(new PaginationHeader(
                users.CurrentPage,
                users.PageSize,
                users.TotalCount,
                users.TotalPages));

            return Ok(new
            {
                total = users.TotalCount,
                data = users
            });
        }

        [HttpGet("{idUser:int}")]
        public async Task<ActionResult<UserResponseDTO>> Get(int idUser)
        {
            var user = await _userService.GetUserById(idUser);
            if (user == null)
                return NotFound(new { message = "Usuário não encontrado." });

            return Ok(user);
        }

        [HttpPut]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> Update([FromBody] UpdateUserDTO updateUserDTO)
        {
            var userUpdated = await _userService.UpdateUser(updateUserDTO);
            if (userUpdated == null)
                return BadRequest(new { message = "Erro ao atualizar usuário." });

            return Ok(new { message = "Usuário atualizado com sucesso.", data = userUpdated });
        }

        [HttpDelete("{idUser:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> Delete(int idUser)
        {
            var user = await _userService.DeleteUser(idUser);
            if (user == null)
                return BadRequest(new { message = "Erro ao excluir usuário." });

            return Ok(new { message = "Usuário excluído com sucesso." });
        }
    }
}
