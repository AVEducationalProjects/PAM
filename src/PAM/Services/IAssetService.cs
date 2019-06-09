using PAM.AssetService.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PAM.Services
{
    public interface IAssetService
    {
        Task<IList<AssetDTO>> GetAllAssetsAsync();

        Task<AssetDTO> CreateAsync(AssetDTO asset);
    }
}
