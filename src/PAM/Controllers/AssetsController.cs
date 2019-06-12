using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAM.AssetService.DTO;
using PAM.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PAM.Controllers
{
    [Authorize]
    public class AssetsController : AuthorizedController
    {
        private readonly IAssetService _assetService;

        public AssetsController(IUserService userService, IAssetService assetService) : base(userService) =>
            _assetService = assetService;

        public async Task<IActionResult> Index()
        {
            IList<AssetDTO> assets = await _assetService.GetAllAssetsAsync();
            return View(assets);
        }

        public async Task<IActionResult> AddAsset(AssetDTO asset)
        {
            await _assetService.CreateAsync(asset);
            return PartialView("AssetList", await _assetService.GetAllAssetsAsync());
        }

    }
}