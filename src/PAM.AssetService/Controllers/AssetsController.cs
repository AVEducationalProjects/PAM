using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PAM.AssetService.DTO;
using PAM.AssetService.Model;
using PAM.AssetService.Services;

namespace PAM.AssetService.Controllers
{
    [Route("/assets")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAssetRepositary _assetRepositary;

        public AssetsController(IMapper mapper, IAssetRepositary assetRepositary)
        {
            _mapper = mapper;
            _assetRepositary = assetRepositary;
        }

        [HttpGet]
        [Route("/assets")]
        [AllowAnonymous]
        public async Task<ActionResult<AssetDTO[]>> Get()
        {
            return _mapper.Map<AssetDTO[]>(await _assetRepositary.GetAll());
        }

        [HttpPost]
        [Route("/assets")]
        [AllowAnonymous]
        public async Task<ActionResult<AssetDTO>> Post(AssetDTO user)
        {
            var storedAsset = await _assetRepositary.Create(_mapper.Map<Asset>(user));
            return Created($"/assets/{storedAsset.Id}", _mapper.Map<AssetDTO>(storedAsset));
        }
    }
}