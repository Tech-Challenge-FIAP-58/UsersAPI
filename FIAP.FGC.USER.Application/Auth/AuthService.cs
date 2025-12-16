
using FIAP.FGC.USER.Application.Services;
using FIAP.FGC.USER.Core.Inputs;
using FIAP.FGC.USER.Core.Validation;
using FIAP.FGC.USER.Core.Web;
using FIAP.FGC.USER.Infra.Repository;
using Microsoft.Extensions.Configuration;

namespace FIAP.FGC.USER.Application.Auth
{
	public class AuthService(IAuthRepository repo, IConfiguration config) : BaseService, IAuthService
	{
        private readonly IAuthRepository _repository = repo;
        private readonly IConfiguration _configuration = config;

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
            var user = await _repository.FindByCredentialsAsync(dto);
            if (user == null)
            {
                return Unauthorized<string>("Credenciais inválidas.");
            }

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
                return BadRequest<int>($"Dados de registro inválidos: {ex.Message}");
            }
            var id = await _repository.Create(dto);
            return Created(id, "Usuário registrado com sucesso.");
        }
	}
}
