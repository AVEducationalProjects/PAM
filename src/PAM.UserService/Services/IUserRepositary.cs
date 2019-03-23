using PAM.UserService.Model;

namespace PAM.UserService.Services
{
    public interface IUserRepositary
    {
        User FindByEmail(string email);
        void Create(User user);
        void DeleteByEmail(string email);
        void CreateOrUpdate(User user);
    }
}