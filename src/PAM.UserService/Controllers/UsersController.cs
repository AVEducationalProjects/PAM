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
        public UserDTO Get(string email)
        {
            return _mapper.Map<UserDTO>(
                _userRepositary.FindByEmail(email));
        }

        [HttpPost]
        public void Post(UserDTO user)
        {
            _userRepositary.Create(
                _mapper.Map<User>(user));
        }

        public void Put(UserDTO user)
        {
            _userRepositary.CreateOrUpdate(
                _mapper.Map<User>(user));
        }

        [HttpDelete]
        public void Delete(string email)
        {
            _userRepositary.DeleteByEmail(email);
        }
    }
}
