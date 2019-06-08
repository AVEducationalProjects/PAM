using System.Threading.Tasks;
using PAM.AssetService.Model;

namespace PAM.AssetService.Services
{
    public interface IAssetRepositary
    {
        Task<Asset> Create(Asset asset);
    }
}
