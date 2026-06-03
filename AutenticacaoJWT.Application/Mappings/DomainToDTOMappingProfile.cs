using AutenticacaoJWT.Application.DTO;
using AutenticacaoJWT.Domain.Entities;
using AutoMapper;

namespace AutenticacaoJWT.Application.Mappings
{
    public class DomainToDTOMappingProfile : Profile
    {
        public DomainToDTOMappingProfile()
        {
            CreateMap<User, UserResponseDTO>();
            CreateMap<CreateUserDTO, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Salt, opt => opt.Ignore());
            CreateMap<UpdateUserDTO, User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Salt, opt => opt.Ignore());
        }
    }
}
