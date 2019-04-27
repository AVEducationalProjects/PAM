using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PAM.Options;
using PAM.UserService.DTO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PAM.Services.UserService
{
    public class UserServiceClient : IUserService
    {
        private readonly string _url;
        private readonly HttpClient _httpClient;

        public UserServiceClient(HttpClient httpClient, IOptions<ServicesOptions> options)
        {
            _url = $"{options.Value.UserService}/Users";
            _httpClient = httpClient;
        }

        public async Task<UserDTO> CreateUserAsync(UserDTO userDTO)
        {
            var content = new StringContent(JsonConvert.SerializeObject(userDTO), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync(_url, content);
            return JsonConvert.DeserializeObject<UserDTO>(await result.Content.ReadAsStringAsync());
        }

        public async Task<UserDTO> GetUserByEmailAsync(string email)
        {
            var result = await _httpClient.GetAsync($"{_url}?email={email}");

            if (result.StatusCode == HttpStatusCode.NotFound)
                return null;

            return JsonConvert.DeserializeObject<UserDTO>(await result.Content.ReadAsStringAsync());
        }
    }
}
