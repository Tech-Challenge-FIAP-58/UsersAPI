using AutoMapper;
using FCG.Application.Inputs;
using FCG.Application.Web;
using FCG.Core.Interfaces.Repository;
using FCG.Core.Interfaces.Utils;

namespace FCG.Application.Services
{
    public class UserApplicationService(IPasswordHasher passwordHasher, IUserRepository repository, IMapper mapper) : BaseService, IUserApplicationService
    {
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IUserRepository _userRepository = repository;
        private readonly IMapper _mapper = mapper;
        public async Task<IApiResponse<IEnumerable<UserResponseDto>>> GetAll()
        {
            var users = await _userRepository.GetAll();
            var result = _mapper.Map<IEnumerable<UserResponseDto>>(users);
            return Ok(result);
        }

        public async Task<IApiResponse<UserResponseDto?>> GetById(int id)
        {
            var user = await _userRepository.GetById(id);

            if (user is null)
                return NotFound<UserResponseDto?>("Usuário não encontrado.");

            var dto = _mapper.Map<UserResponseDto>(user);

            return Ok<UserResponseDto?>(dto);
        }

        public async Task<IApiResponse<bool>> Update(int id, UserUpdateDto dto)
        {
            var user = await _userRepository.GetById(id);
            if (user == null) 
                return NotFound<bool>("Usuário não encontrado para atualização.");
            
            _mapper.Map(dto, user);

            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.Password = _passwordHasher.Hash(dto.Password);

            await _userRepository.Update(id, user);
            return NoContent();
        }

        public async Task<IApiResponse<bool>> Remove(int id)
        {
            var user = await _userRepository.GetById(id);

            if (user == null)
                return NotFound<bool>("Usuário não encontrado para remoção.");

            await _userRepository.Remove(user);

            return NoContent();
        }
    }
}