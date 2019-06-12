using System.Linq;
using System.Security.Claims;

namespace PAM.Helpers
{
    public static class UserHelpers
    {
        public static string GetFullName(this ClaimsPrincipal user) => user.Claims.Single(x => x.Type == "FullName").Value;
    }
}
