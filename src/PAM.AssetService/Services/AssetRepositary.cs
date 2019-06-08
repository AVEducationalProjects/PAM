using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PAM.AssetService.Model;
using PAM.AssetService.Options;

namespace PAM.AssetService.Services
{
    public class AssetRepositary : IAssetRepositary
    {
        private IMongoCollection<Asset> Assets { get; }
        private ILogger<AssetRepositary> Logger { get; }

        public AssetRepositary(ILogger<AssetRepositary> logger, IOptions<MongoOptions> mongoOptions)
        {
            Logger = logger;

            var client = new MongoClient(mongoOptions.Value.Server);
            var database = client.GetDatabase(mongoOptions.Value.Database);
            Assets = database.GetCollection<Asset>(mongoOptions.Value.AssetCollection);
        }

        public async Task<Asset> Create(Asset asset)
        {
            await Assets.InsertOneAsync(asset);
            return asset;
        }
    }
}
