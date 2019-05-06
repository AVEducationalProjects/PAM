using PAM.UserService.Model;
using System.Threading.Tasks;

namespace PAM.UserService.Services
{
    public interface IHouseholdRepositary
    {
        Task<Household> Create(User user, Household household);
    }
}
