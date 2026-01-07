using System.Net;
using UserService.Application.Inputs;
using UserService.Application.Web;

namespace UserService.Application.Auth
{
    public interface IAuthService
    {
        Task<IApiResponse<string>> Login(LoginDto dto);
        Task<IApiResponse<int>> Register(UserRegisterDto dto);
        Task<IApiResponse<int>> RegisterAdmin(UserRegisterDto dto);
    }
}