
using FIAP.FGC.USER.Core.Inputs;
using FIAP.FGC.USER.Core.Web;

namespace FIAP.FGC.USER.Application.Services
{
	public interface IUserService
	{
		Task<IApiResponse<IEnumerable<UserResponseDto>>> GetAll();
        Task<IApiResponse<UserResponseDto?>> GetById(int id);
        Task<IApiResponse<bool>> Update(int id, UserUpdateDto userUpdateDto);
        Task<IApiResponse<bool>> Remove(int id);
    }
}
