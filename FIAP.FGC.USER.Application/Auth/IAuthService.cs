using FIAP.FGC.USER.Core.Inputs;
using FIAP.FGC.USER.Core.Web;
using System.Net;

namespace FIAP.FGC.USER.Application.Auth
{
	public interface IAuthService
	{
		Task<IApiResponse<string>> Login(LoginDto dto);
        Task<IApiResponse<int>> Register(UserRegisterDto dto);
    }
}
