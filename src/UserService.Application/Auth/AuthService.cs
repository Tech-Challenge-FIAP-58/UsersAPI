using AutoMapper;
using Microsoft.Extensions.Configuration;
using UserService.Application.Inputs;
using UserService.Application.Services;
using UserService.Application.Validation;
using UserService.Application.Web;
using UserService.Core.Interfaces.Repository;
using UserService.Core.Interfaces.Utils;
using UserService.Core.Models;

namespace UserService.Application.Auth
{
    public class AuthService(IMapper mapper, IPasswordHasher passwordHasher, IAuthRepository repo, IConfiguration config) : BaseService, IAuthService
    {
        private readonly IAuthRepository _repository = repo;
        private readonly IConfiguration _configuration = config;
        private readonly IMapper _mapper = mapper;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<IApiResponse<string>> Login(LoginDto dto)
        {
            try
            {
                DtoValidator.ValidateObject(dto);
            }
            catch (Exception ex)
            {
                return BadRequest<string>($"Dados de login inválidos: {ex.Message}");
            }

            var user = await _repository.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized<string>("Credenciais inválidas.");

            var senhaEstaValida = _passwordHasher.Verify(dto.Password, user.Password);
            if (!senhaEstaValida)
                return Unauthorized<string>("Credenciais inválidas.");

            var token = _repository.GenerateToken(_configuration, user);
            return Ok(token);
        }

        public async Task<IApiResponse<int>> Register(UserRegisterDto dto)
        {
            try
            {
                DtoValidator.ValidateObject(dto);
            }
            catch (Exception ex)
            {
                return BadRequest<int>($"Dados inválidos: {ex.Message}");
            }

            if (await _repository.ExistsByEmailAsync(dto.Email))
                return Conflict<int>("E-mail já cadastrado.");

            if (await _repository.ExistsByCpfAsync(dto.Cpf))
                return Conflict<int>("CPF já cadastrado.");

            var entity = _mapper.Map<User>(dto);
            entity.Password = _passwordHasher.Hash(dto.Password);

            var id = await _repository.Create(entity);
            return Created(id, "Usuário registrado com sucesso.");
        }

        public async Task<IApiResponse<int>> RegisterAdmin(UserRegisterDto dto)
        {
            try
            {
                DtoValidator.ValidateObject(dto);
            }
            catch (Exception ex)
            {
                return BadRequest<int>($"Dados inválidos: {ex.Message}");
            }

            if (await _repository.ExistsByEmailAsync(dto.Email))
                return Conflict<int>("E-mail já cadastrado.");

            if (await _repository.ExistsByCpfAsync(dto.Cpf))
                return Conflict<int>("CPF já cadastrado.");

            var entity = _mapper.Map<User>(dto);
            entity.Password = _passwordHasher.Hash(dto.Password);
            entity.IsAdmin = true;

            var id = await _repository.Create(entity);
            return Created(id, "Administrador registrado com sucesso.");
        }
    }
}