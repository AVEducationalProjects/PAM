using Newtonsoft.Json;
using PAM.UserService.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PAM.Helpers
{
    public static class UserExtensions
    {
        public static IList<HouseholdDTO> GetHouseholds(this ClaimsPrincipal user)
        {
            return user.Claims
                .Where(x => x.Type == "Households")
                .Select(x => JsonConvert.DeserializeObject<HouseholdDTO>(x.Value))
                .ToList();
        }
    }
}
