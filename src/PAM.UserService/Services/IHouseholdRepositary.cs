using MongoDB.Bson;
using PAM.UserService.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PAM.UserService.Services
{
    public interface IHouseholdRepositary
    {
        Task<Household> Create(User user, Household household);
        Task<Household[]> FindHouseholdsById(IEnumerable<ObjectId> households);
        Task RemoveUserHousehold(User user, ObjectId householdId);
        Task AddHouseholdToUser(User user, ObjectId id);
    }
}
