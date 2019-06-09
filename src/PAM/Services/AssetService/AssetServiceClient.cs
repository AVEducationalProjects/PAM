using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PAM.AssetService.DTO;
using PAM.Options;

namespace PAM.Services.AssetService
{
    public class AssetServiceClient : IAssetService
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;

        public AssetServiceClient(HttpClient httpClient, IOptions<ServicesOptions> options)
        {
            _httpClient = httpClient;
            _url = $"{options.Value.AssetService}/assets";
        }

        public async Task<AssetDTO> CreateAsync(AssetDTO asset)
        {
            var content = new StringContent(JsonConvert.SerializeObject(asset), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync(_url, content);
            return JsonConvert.DeserializeObject<AssetDTO>(await result.Content.ReadAsStringAsync());
        }

        public async Task<IList<AssetDTO>> GetAllAssetsAsync()
        {
            var result = await _httpClient.GetAsync(_url);
            return JsonConvert.DeserializeObject<AssetDTO[]>(
                await result.Content.ReadAsStringAsync());
        }
    }
}
