using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PAM.Exceptions;
using PAM.Options;
using PAM.Services;
using PAM.UserService.DTO;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

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

            //var jwt = CreateJWT(userInfo);

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, userInfo.Email),
                //new Claim("JWT", jwt)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { });

            return RedirectToAction("Index", "Assets");
        }

    }
}