using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using PAM.Infrastructure.Options;
using PAM.UserService.DTO;
using PAM.UserService.Model;
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
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;
        private readonly IUserRepositary _userRepositary;
        private readonly IHouseholdRepositary _householdRepositary;
        private readonly JWTSigninOptions _jwtOptions;

        public UsersController(
            IAuthorizationService authorizationService,
            IMapper mapper, 
            IUserRepositary userRepositary, 
            IHouseholdRepositary householdRepositary, 
            IOptions<JWTSigninOptions> jwtOptions)
        {
            _authorizationService = authorizationService;
            _mapper = mapper;
            _userRepositary = userRepositary;
            _householdRepositary = householdRepositary;
            _jwtOptions = jwtOptions.Value;
        }


        [HttpPost]
        [Route("/users")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDTO>> Post(UserDTO user)
        {
            var storedUser = await _userRepositary.Create(_mapper.Map<User>(user));

            await _householdRepositary.Create(storedUser, new Household { Name = "Default" });

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

            return Created("", result);
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
                EncryptingCredentials = new X509EncryptingCredentials(new X509Certificate2(_jwtOptions.EncryptionCertificate))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<bool> UserCanAccessProfile(User profile)
        {
            var authorizeResult = await _authorizationService.AuthorizeAsync(User, profile, "UserProfilePolicy");
            return authorizeResult.Succeeded;
        }

        [HttpGet]
        [Route("/users/{email}")]
        public async Task<ActionResult<UserDTO>> Get([FromRoute]string email)
        {
            var storedUser = await _userRepositary.FindByEmail(email);

            if (storedUser == null)
                return NotFound();

            if (!await UserCanAccessProfile(storedUser))
                return Forbid();

            return Ok(_mapper.Map<UserDTO>(storedUser));
        }

        [HttpPatch]
        [Route("/users/{email}")]
        public async Task<ActionResult> Patch([FromRoute]string email, UserPatchDTO userPatch)
        {
            var storedUser = await _userRepositary.FindByEmail(email);

            if (storedUser == null)
                return NotFound();

            if (!await UserCanAccessProfile(storedUser))
                return Forbid();

            _mapper.Map(userPatch, storedUser);

            await _userRepositary.Update(storedUser);

            return NoContent();
        }

        [HttpGet]
        [Route("/users/{email}/households")]
        public async Task<ActionResult<HouseholdDTO[]>> GetHouseholds([FromRoute]string email)
        {
            var storedUser = await _userRepositary.FindByEmail(email);

            if (storedUser == null)
                return NotFound();

            if (!await UserCanAccessProfile(storedUser))
                return Forbid();

            var result = await _householdRepositary.FindHouseholdsById(storedUser.Households);

            return Ok(result);
        }

        [HttpPost]
        [Route("/users/{email}/households")]
        public async Task<ActionResult<HouseholdDTO[]>> PostHousehold([FromRoute]string email, HouseholdDTO household)
        {
            var storedUser = await _userRepositary.FindByEmail(email);

            if (storedUser == null)
                return NotFound();

            if (!await UserCanAccessProfile(storedUser))
                return Forbid();

            var result = await _householdRepositary.Create(storedUser, _mapper.Map<Household>(household));

            return Created($"/users/{email}/households/{result.Id}", result);
        }

        [HttpDelete]
        [Route("/users/{email}/households/{id}")]
        public async Task<ActionResult> DeleteHousehold([FromRoute]string email, [FromRoute]string id)
        {
            var storedUser = await _userRepositary.FindByEmail(email);

            if (storedUser == null)
                return NotFound();

            if (!await UserCanAccessProfile(storedUser))
                return Forbid();

            await _householdRepositary.RemoveUserHousehold(storedUser, ObjectId.Parse(id));

            return NoContent();
        }

        [HttpPost]
        [Route("/users/{email}/households/{id}/actions/share")]
        public async Task<ActionResult> ShareHousehold([FromRoute]string email, [FromRoute]string id, ShareHouseholdDTO shareHousehold)
        {
            var storedUser = await _userRepositary.FindByEmail(email);

            if (storedUser == null)
                return NotFound();

            if (!await UserCanAccessProfile(storedUser))
                return Forbid();

            var user = await _userRepositary.FindByEmail(shareHousehold.UserToShareWith);

            await _householdRepositary.AddHouseholdToUser(user, ObjectId.Parse(id));

            return Accepted();
        }

    }
}
