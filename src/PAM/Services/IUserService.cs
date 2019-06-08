using PAM.UserService.DTO;
using System.Threading.Tasks;

namespace PAM.Services
{
    public interface IUserService
    {
        void UseToken(string jwt);

        Task<UserDTO> GetUserByEmailAsync(string email);
        Task<UserDTO> CreateUserAsync(UserDTO userDTO);
        Task<TokenDTO> GetUserTokenAsync(string email);

        Task<HouseholdDTO[]> GetUserHouseholdsAsync(string email);
        Task AddHousehold(string email, string name);
    }
}