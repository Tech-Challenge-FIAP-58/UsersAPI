using UserService.Application.Inputs;
using UserService.Application.Web;

namespace UserService.Application.Services
{
    public interface IUserApplicationService
    {
        Task<IApiResponse<IEnumerable<UserResponseDto>>> GetAll();
        Task<IApiResponse<UserResponseDto?>> GetById(int id);
        Task<IApiResponse<bool>> Update(int id, UserUpdateDto userUpdateDto);
        Task<IApiResponse<bool>> Remove(int id);
    }
}