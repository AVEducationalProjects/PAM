using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAM.Services;
using PAM.UserService.DTO;
using System.Collections.Generic;
using System.Linq;

namespace PAM.Controllers
{
    public abstract class AuthorizedController : Controller
    {
        protected readonly IUserService _userService;

        public AuthorizedController(IUserService userService) =>
            _userService = userService;

        protected IList<HouseholdDTO> GetHouseholds()
        {
            return JsonConvert.DeserializeObject<HouseholdDTO[]>(
                HttpContext.Session.GetString("Households")).ToList();
        }

        protected string GetCurrentHouseholdId() => HttpContext.Session.GetString("CurrentHouseholdId");

        public override ViewResult View(string viewName, object model)
        {
            IList<HouseholdDTO> availableHouseholds = GetHouseholds();
            var currentHouseholdId = GetCurrentHouseholdId();

            HouseholdDTO selectedHousehold = availableHouseholds.Single(x => x.Id == currentHouseholdId);
            availableHouseholds.Remove(selectedHousehold);

            ViewBag.HeaderDTO = (Available: availableHouseholds, Selected: selectedHousehold);
            return base.View(viewName, model);
        }
    }
}