using AutenticacaoJWT.Application.DTO;
using AutenticacaoJWT.Application.Interfaces;
using AutenticacaoJWT.Domain.Entities;
using AutenticacaoJWT.Domain.Interfaces;
using AutenticacaoJWT.Domain.Pagination;
using AutoMapper;

namespace AutenticacaoJWT.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
            IGenericRepository<User> userRepository,
            IMapper mapper,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<PagedList<UserResponseDTO>> GetAllUsers(int pageNumber, int pageSize)
        {
            var users = await _userRepository.GetAllPagination(pageNumber, pageSize);
            var usersDTO = _mapper.Map<List<UserResponseDTO>>(users);

            return new PagedList<UserResponseDTO>(usersDTO, pageNumber, pageSize, users.TotalCount);
        }

        public async Task<UserResponseDTO?> GetUserById(int idUser)
        {
            var user = await _userRepository.GetById(idUser);
            if (user == null)
                return null;

            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<UserResponseDTO?> AddUser(CreateUserDTO createUserDTO)
        {
            var user = _mapper.Map<User>(createUserDTO);
            user.Salt = _passwordHasher.GenerateSalt();
            user.Password = _passwordHasher.HashPassword(createUserDTO.Password, user.Salt);
            user.IsAdmin = false;

            var newUser = await _userRepository.Create(user);
            return _mapper.Map<UserResponseDTO>(newUser);
        }

        public async Task<UserResponseDTO?> UpdateUser(UpdateUserDTO updateUserDTO)
        {
            var user = await _userRepository.GetById(updateUserDTO.Id);
            if (user == null)
                return null;

            user.Name = updateUserDTO.Name;
            user.Email = updateUserDTO.Email;
            user.IsAdmin = updateUserDTO.IsAdmin;

            if (!string.IsNullOrWhiteSpace(updateUserDTO.Password))
            {
                user.Salt = _passwordHasher.GenerateSalt();
                user.Password = _passwordHasher.HashPassword(updateUserDTO.Password, user.Salt);
            }

            var userUpdate = await _userRepository.Update(user);
            return _mapper.Map<UserResponseDTO>(userUpdate);
        }

        public async Task<UserResponseDTO?> DeleteUser(int idUser)
        {
            var user = await _userRepository.GetById(idUser);
            if (user == null)
                return null;

            await _userRepository.Remove(user);
            return _mapper.Map<UserResponseDTO>(user);
        }
    }
}
