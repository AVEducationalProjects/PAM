using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PAM.UserService.DTO;
using PAM.UserService.Model;
using PAM.UserService.Services;

namespace PAM.UserService.Controllers
{
    [Route("/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepositary _userRepositary;

        public UsersController(IMapper mapper, IUserRepositary repositary)
        {
            _mapper = mapper;
            _userRepositary = repositary;
        }

        [HttpGet]
        public ActionResult<UserDTO> Get(string email)
        {
            var result = _mapper.Map<UserDTO>(
                _userRepositary.FindByEmail(email));

            if (result == null)
                return NotFound();

            return result;
        }

        [HttpPost]
        public ActionResult<UserDTO> Post(UserDTO user)
        {
            return _mapper.Map<UserDTO>(
                _userRepositary.Create(
                    _mapper.Map<User>(user)));
        }

        public ActionResult<UserDTO> Put(UserDTO user)
        {
            return _mapper.Map<UserDTO>(
                _userRepositary.CreateOrUpdate(
                    _mapper.Map<User>(user)));
        }

        [HttpDelete]
        public void Delete(string email)
        {
            _userRepositary.DeleteByEmail(email);
        }
    }
}
