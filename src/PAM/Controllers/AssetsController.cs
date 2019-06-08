using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAM.Services;

namespace PAM.Controllers
{
    [Authorize]
    public class AssetsController : Controller
    {
        public IUserService UserService { get; set; }

        public AssetsController(IUserService userService)
        {
            UserService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddHousehold(string name)
        {
            UserService.AddHousehold(User.Claims.First(x => x.Type == "JWT").Value, name);

            return RedirectToAction("Index");
        }
    }
}