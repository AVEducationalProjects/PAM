using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAM.Exceptions;
using PAM.Services;
using PAM.UserService.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PAM.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private readonly IFacebookService _facebookService;

        private readonly IUserService _userService;

        public AuthController(IFacebookService facebook, IUserService userService) =>
            (_facebookService, _userService) = (facebook, userService);

        [AllowAnonymous]
        [Route("/SignInViaFacebook")]
        public async Task<IActionResult> SignInViaFacebook(string token)
        {
            FacebookUserProfile profile = await _facebookService.GetUserInfoAsync(token);

            if (profile == null)
                throw new AuthException("Error login via Facebook.");

            UserDTO userInfo = await _userService.GetUserByEmailAsync(profile.Email);

            if (userInfo == null)
                userInfo = await _userService.CreateUserAsync(
                    new UserDTO
                    {
                        Email = profile.Email,
                        Name = profile.Name
                    });

            await SignInAsUser(userInfo);

            await UpdateHouseholds(userInfo.Email);

            return RedirectToAction("Index", "Assets");
        }

        private async Task UpdateHouseholds(string userName, string householdId = null)
        {
            HouseholdDTO[] households = await _userService.GetUserHouseholdsAsync(userName);

            if (householdId != null && !households.Any(x => x.Id == householdId))
                throw new ApplicationException("User hasn't permission for such household.");

            HttpContext.Session.SetString("Households", JsonConvert.SerializeObject(households.ToArray()));
            HttpContext.Session.SetString("CurrentHouseholdId", householdId ?? households.First().Id);
        }

        private async Task SignInAsUser(UserDTO userInfo)
        {
            TokenDTO jwt = await _userService.GetUserTokenAsync(userInfo.Email);
            _userService.UseToken(jwt.Token);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userInfo.Email),
                new Claim("FullName", userInfo.Name),
                new Claim("JWT", jwt.Token),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { });
        }

        [Route("User/ChangeHousehold/{id}")]
        public async Task<IActionResult> ChangeHousehold([FromRoute]string id)
        {
            await UpdateHouseholds(User.Identity.Name, id);
            return RedirectToAction("Index", "Assets");
        }
    }
}