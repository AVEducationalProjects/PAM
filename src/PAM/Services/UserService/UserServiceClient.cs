using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PAM.Options;
using PAM.UserService.DTO;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PAM.Services.UserService
{
    public class UserServiceClient : IUserService
    {
        private string _bearer = null;
        private readonly string _url;
        private readonly HttpClient _httpClient;

        private void AddAuthorizationHeader()
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", _bearer);
        }

        public void UseToken(string jwt)
        {
            _bearer = $"Bearer {jwt}";
        }

        public UserServiceClient(HttpClient httpClient, IOptions<ServicesOptions> options)
        {
            _url = $"{options.Value.UserService}/users";
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
            var result = await _httpClient.GetAsync($"{_url}/{email}");

            if (result.StatusCode == HttpStatusCode.NotFound)
                return null;

            return JsonConvert.DeserializeObject<UserDTO>(await result.Content.ReadAsStringAsync());
        }

        public async Task<TokenDTO> GetUserTokenAsync(string email)
        {
            var result = await _httpClient.PostAsync($"{_url}/{email}/actions/get-token", new StringContent(string.Empty));
            return JsonConvert.DeserializeObject<TokenDTO>(await result.Content.ReadAsStringAsync());
        }

        public async Task<HouseholdDTO[]> GetUserHouseholdsAsync(string email)
        {
            AddAuthorizationHeader();
            var result = await _httpClient.GetAsync($"{_url}/{email}/households");
            return JsonConvert.DeserializeObject<HouseholdDTO[]>(await result.Content.ReadAsStringAsync());
        }

        public async Task AddHousehold(string email, string name)
        {
            AddAuthorizationHeader();
            var content = new StringContent("{'name' : '" + name + "'}", Encoding.UTF8, "application/json");
            await _httpClient.PostAsync($"{_url}/{email}/households", content);
        }
    }
}
