using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PAM.AssetService.DTO;
using PAM.AssetService.Services;

namespace PAM.AssetService.Controllers
{
    [Route("/assettypes")]
    [ApiController]
    public class AssetTypesController : ControllerBase
    {
        private readonly IAssetRepositary _assetRepositary;

        public AssetTypesController(IAssetRepositary assetRepositary)
        {
            _assetRepositary = assetRepositary;
        }

        [Route("/assettypes")]
        [HttpPost]
        public async Task<ActionResult<AssetTypeDTO>> Post(AssetTypeDTO assetType)
        {
            return Created($"/assettypes/{assetType.Id}", assetType);
        }

        [Route("/assettypes")]
        [HttpGet]
        public async Task<ActionResult<AssetTypeDTO[]>> Get()
        {
            return Ok(new AssetTypeDTO[] { });
        }
    }
}