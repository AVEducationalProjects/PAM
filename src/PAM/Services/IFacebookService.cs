using System.Threading.Tasks;

namespace PAM.Services
{
    public interface IFacebookService
    {
        Task<FacebookUserProfile> GetUserInfoAsync(string token);
    }

    public class FacebookUserProfile
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}