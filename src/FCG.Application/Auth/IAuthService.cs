using FCG.Application.Inputs;
using FCG.Application.Web;
using System.Net;

namespace FCG.Application.Auth
{
    public interface IAuthService
    {
        Task<IApiResponse<string>> Login(LoginDto dto);
        Task<IApiResponse<int>> Register(UserRegisterDto dto);
        Task<IApiResponse<int>> RegisterAdmin(UserRegisterDto dto);
    }
}