using AutoMapper;
using FCG.Application.Inputs;
using FCG.Core.Models;

namespace FCG.Application.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // REGISTER: DTO -> Entity (via factory)
        CreateMap<UserRegisterDto, User>()
            .ConvertUsing((dto, _, _) =>
                User.Create(
                    dto.Name,
                    dto.Email,
                    dto.Password,
                    dto.Cpf,
                    dto.Address,
                    false
                ));
        // Password será hasheada no service antes de persistir

        // UPDATE: DTO -> Entity (aplica apenas quando vier valor)
        CreateMap<UserUpdateDto, User>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Cpf, opt => opt.Ignore())      // não permitir alterar CPF por update
            .ForMember(d => d.IsAdmin, opt => opt.Ignore())  // evitar escalonamento por esse DTO
            .ForMember(d => d.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(d => d.Notifications, opt => opt.Ignore())
            .ForMember(d => d.Password, opt => opt.Ignore()) // troca de senha via DTO próprio + hash
            .ForMember(d => d.UserRoles, opt => opt.Ignore())// navegação gerenciada fora do AutoMapper
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Entity -> DTO de resposta
        CreateMap<User, UserResponseDto>();
    }
}