using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAM.AssetService.DTO;
using PAM.Services;

namespace PAM.Controllers
{
    [Authorize]
    public class AssetsController : Controller
    {
        public IUserService UserService { get; set; }

        public IAssetService AssetService { get; set; }

        public AssetsController(IUserService userService, IAssetService assetService)
        {
            UserService = userService;
            AssetService = assetService;
        }

        public async Task<IActionResult> Index()
        {
            var assets = await AssetService.GetAllAssetsAsync();
            return View(assets);
        }

        public async Task<IActionResult> AddAsset(AssetDTO asset)
        {
            await AssetService.CreateAsync(asset);
            return PartialView("AssetList", await AssetService.GetAllAssetsAsync());
        }

        public IActionResult AddHousehold(string name)
        {
            UserService.AddHousehold(User.Claims.First(x => x.Type == "JWT").Value, name);

            return RedirectToAction("Index");
        }
    }
}