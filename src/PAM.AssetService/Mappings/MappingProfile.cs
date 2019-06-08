using AutoMapper;
using PAM.AssetService.DTO;
using PAM.AssetService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PAM.AssetService.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AssetDTO, Asset>();
            CreateMap<Asset, AssetDTO>();
        }
    }
}
