using AutoMapper;
using FIAP.FGC.USER.Core.Inputs;
using FIAP.FGC.USER.Core.Models;
using FIAP.FGC.USER.Infra.Context;

namespace FIAP.FGC.USER.Infra.Repository
{
    public class UserRepository(ApplicationDbContext context, IMapper mapper) : EFRepository<User>(context), IUserRepository
    {
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<UserResponseDto>> GetAll()
        {
            var users = await Get();
            return [.. users.Select(u => _mapper.Map<UserResponseDto>(u))];
        }

        public async Task<UserResponseDto?> GetById(int id)
        {
            var user = await Get(id);
            return user is null ? null : _mapper.Map<UserResponseDto>(user);
        }

        public async Task<bool> Update(int id, UserUpdateDto userUpdateDto)
        {
            var user = await Get(id) ?? throw new ArgumentNullException(nameof(id), $"Erro ao atualizar: Usuário inexistente!");
            _mapper.Map(userUpdateDto, user);
            if (!string.IsNullOrWhiteSpace(userUpdateDto.Password))
                user.Password = AuthRepository.HashPassword(userUpdateDto.Password);
            return await Edit(user);
        }
        public async Task<bool> Remove(int id)
        {
            return await Delete(id);
        }
    }
}
