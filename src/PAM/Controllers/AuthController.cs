using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAM.Exceptions;
using PAM.Services;
using PAM.UserService.DTO;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace PAM.Controllers
{
    public class AuthController : Controller
    {
        private readonly IFacebookService _facebookService;

        private readonly IUserService _userService;

        public AuthController(IFacebookService facebook, IUserService userService)
        {
            _facebookService = facebook;
            _userService = userService;
        }

        [Route("/SignInViaFacebook")]
        public async Task<IActionResult> SignInViaFacebook(string token)
        {
            var profile = await _facebookService.GetUserInfoAsync(token);

            if (profile == null)
                throw new AuthException("Error login via Facebook.");

            var userInfo = await _userService.GetUserByEmailAsync(profile.Email);

            if (userInfo == null)
                userInfo = await _userService.CreateUserAsync(
                    new UserDTO
                    {
                        Email = profile.Email,
                        Name = profile.Name
                    });

            await SignInAsUser(userInfo);

            return RedirectToAction("Index", "Assets");
        }

        private async Task SignInAsUser(UserDTO userInfo)
        {
            var jwt = await _userService.GetUserTokenAsync(userInfo.Email);
            _userService.UseToken(jwt.Token);

            var households = await _userService.GetUserHouseholdsAsync(userInfo.Email);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userInfo.Email),
                new Claim("JWT", jwt.Token),
            };

            claims.AddRange(households.Select(h => new Claim("Households", JsonConvert.SerializeObject(h))));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { });
        }
    }
}