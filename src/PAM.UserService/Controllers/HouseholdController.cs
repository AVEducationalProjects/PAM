using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAM.UserService.Model;
using PAM.UserService.Services;

namespace PAM.UserService.Controllers
{
    [Authorize]
    [Route("/Households")]
    [ApiController]
    public class HouseholdController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepositary _userRepositary;
        private readonly IHouseholdRepositary _householdRepositary;

        public HouseholdController(IMapper mapper, IUserRepositary userRepositary, IHouseholdRepositary householdRepositary)
        {
            _mapper = mapper;
            _userRepositary = userRepositary;
            _householdRepositary = householdRepositary;
        }

        [HttpPost]
        public async void Post(string name)
        {
            var user = await _userRepositary.FindByEmail(User.Identity.Name);
            await _householdRepositary.Create(user, new Household { Name = name});
        }
    }
}