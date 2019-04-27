using PAM.UserService.Model;

namespace PAM.UserService.Services
{
    public interface IHouseholdRepositary
    {
        Household Create(User user, Household household);
    }
}
