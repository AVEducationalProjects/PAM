using PAM.UserService.Model;

namespace PAM.UserService.Services
{
    public interface IUserRepositary
    {
        User FindByEmail(string email);
        User Create(User user);
        void DeleteByEmail(string email);
        User CreateOrUpdate(User user);
    }
}