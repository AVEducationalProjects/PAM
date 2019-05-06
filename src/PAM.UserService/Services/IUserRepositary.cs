using PAM.UserService.Model;
using System.Threading.Tasks;

namespace PAM.UserService.Services
{
    public interface IUserRepositary
    {
        Task<User> FindByEmail(string email);
        Task<User> Create(User user);
        Task<User> Update(string email, User user);
    }
}