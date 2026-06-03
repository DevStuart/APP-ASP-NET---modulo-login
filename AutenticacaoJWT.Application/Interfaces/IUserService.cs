using AutenticacaoJWT.Application.DTO;
using AutenticacaoJWT.Domain.Pagination;

namespace AutenticacaoJWT.Application.Interfaces
{
    public interface IUserService
    {
        Task<PagedList<UserResponseDTO>> GetAllUsers(int pageNumber, int pageSize);
        Task<UserResponseDTO?> GetUserById(int idUser);
        Task<UserResponseDTO?> AddUser(CreateUserDTO createUserDTO);
        Task<UserResponseDTO?> UpdateUser(UpdateUserDTO updateUserDTO);
        Task<UserResponseDTO?> DeleteUser(int idUser);
    }
}
