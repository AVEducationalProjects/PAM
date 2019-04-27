using PAM.UserService.DTO;
using System.Threading.Tasks;

namespace PAM.Services
{
    public interface IUserService
    {
        Task<UserDTO> GetUserByEmailAsync(string email);
        Task<UserDTO> CreateUserAsync(UserDTO userDTO);
    }
}