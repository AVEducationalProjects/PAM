using AutoMapper;
using PAM.UserService.DTO;
using PAM.UserService.Model;

namespace PAM.UserService.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDTO, User>();
            CreateMap<User, UserDTO>();
        }
    }
}
