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
            CreateMap<UserPatchDTO, User>().ForAllMembers(
                opt => opt.Condition(
                    (source, dest, sourceMember, destMember) => (sourceMember != null)));

            CreateMap<Household, HouseholdDTO>();
            CreateMap<HouseholdDTO, Household>();
        }
    }
}
