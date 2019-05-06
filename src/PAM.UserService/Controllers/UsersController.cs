using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PAM.Infrastructure.Options;
using PAM.UserService.DTO;
using PAM.UserService.Model;
using PAM.UserService.Options;
using PAM.UserService.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace PAM.UserService.Controllers
{
    [ApiController]
    [Route("/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepositary _userRepositary;
        private readonly IHouseholdRepositary _householdRepositary;
        private readonly JWTSigninOptions _jwtOptions;

        public UsersController(IMapper mapper, 
            IUserRepositary userRepositary, 
            IHouseholdRepositary householdRepositary, 
            IOptions<JWTSigninOptions> jwtOptions)
        {
            _mapper = mapper;
            _userRepositary = userRepositary;
            _householdRepositary = householdRepositary;
            _jwtOptions = jwtOptions.Value;
        }

        [HttpGet]
        [Route("/users/{email}")]
        public async Task<ActionResult<UserDTO>> Get([FromRoute]string email)
        {
            var result = _mapper.Map<UserDTO>(
                await _userRepositary.FindByEmail(email));

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserDTO>> Post(UserDTO user)
        {
            var storedUser = await _userRepositary.Create(_mapper.Map<User>(user));

            await _householdRepositary.Create(storedUser, new Household { Name = "Default"});

            return Created($"/users/{user.Email}", _mapper.Map<UserDTO>(storedUser));
        }

        [HttpPost]
        [Route("/users/{email}/actions/get-token")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenDTO>> GetToken([FromRoute]string email)
        {
            var user = _mapper.Map<UserDTO>(
                await _userRepositary.FindByEmail(email));

            if (user == null)
                return NotFound();

            var result = new TokenDTO
            {
                Token = CreateJWT(user)
            };

            return Ok(result);
        }

        private string CreateJWT(UserDTO user)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "PAM",
                Audience = "*.PAM",
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now,
                Expires = DateTime.UtcNow.AddDays(7),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("FullName", user.Name)
                }),
                SigningCredentials = new X509SigningCredentials(new X509Certificate2(_jwtOptions.SigningCertificate, _jwtOptions.SigningPassword)),
                EncryptingCredentials = new X509EncryptingCredentials(new X509Certificate2(_jwtOptions.EncryptionCertificate, _jwtOptions.EncryptionPassword))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
