using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FCG.Application.Auth;
using FCG.Application.Inputs;

namespace FCG.WebApi.Controllers
{
    public class AuthController(IAuthService service, ILogger<AuthController> logger) : StandardController
    {
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            logger.LogInformation("POST - Login para o usuário com email: {Email}", loginDto.Email);
            return await TryMethodAsync(() => service.Login(loginDto), logger);
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterRequestDto)
        {
            logger.LogInformation("POST - Registro de novo usuário com email: {Email}", userRegisterRequestDto.Email);
            return await TryMethodAsync(() => service.Register(userRegisterRequestDto), logger);
        }

        [HttpPost("RegisterAdmin")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterAdmin([FromBody] UserRegisterDto userRegisterRequestDto)
        {
            logger.LogInformation("POST - Registro de novo administrador com email: {Email}", userRegisterRequestDto.Email);
            return await TryMethodAsync(() => service.RegisterAdmin(userRegisterRequestDto), logger);
        }
    }
}