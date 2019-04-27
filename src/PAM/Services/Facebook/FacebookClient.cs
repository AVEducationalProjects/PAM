using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PAM.Services.Facebook
{
    public class FacebookClient : IFacebookService
    {
        private const string FB = "https://graph.facebook.com";

        private readonly HttpClient _httpClient;

        public FacebookClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<FacebookUserProfile> GetUserInfoAsync(string token)
        {
            var url = $"{FB}/me?fields=id,name,email&access_token={token}";
            var response = await _httpClient.GetAsync(url);
            var result = JsonConvert.DeserializeObject<FacebookUserProfile>(await response.Content.ReadAsStringAsync());

            return result;
        }

    }
}
