using AuthenticationApi.Application.DTOs;
using ECommerce.SharedLibrary.Responses;

namespace AuthenticationApi.Application.Interface
{
    public interface IUser
    {
        Task<Response> Register(AppUserDTO userDTO);
        Task<Response> Login(LoginDTO loginDTO);
        Task<GetUserDTO> GetUser(int userId);
    }
}
