using Microsoft.AspNetCore.Mvc;
using PAM.UserService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PAM.UserService.Controllers
{
    [Route("/Users")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public User Get(string email)
        {
            return new User
            {
                Email = email
            };
        }

        [HttpPost]
        public void Post(User user)
        {
        }
    }
}
