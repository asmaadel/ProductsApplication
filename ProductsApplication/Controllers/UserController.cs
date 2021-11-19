using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ProductsApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings jwtSettings;

        public UserController(IMapper mapper, JwtSettings settings, UserManager<User> userManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            jwtSettings = settings;
        }
        [HttpGet("Test")]
        [AllowAnonymous]
        public String Test()
        {
            return "accounts controller";
        }
        [HttpPost("Register")]
        public async Task<ActionResult> Register(RegisterDto userModel)
        {
            var user = _mapper.Map<User>(userModel);
            user.CreatedDate = DateTime.Now;
            var result = await _userManager.CreateAsync(user, userModel.Password);
            if (!result.Succeeded)
            {
                return Ok(result.Errors);
            }
            //await _userManager.AddToRoleAsync(user, "Visitor");

            return StatusCode(201);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginModel userModel)
        {
            var user = await _userManager.FindByEmailAsync(userModel.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, userModel.Password))
            {               
                var signingCredentials = GetSigningCredentials();
                var claims = GetClaims(user);
                var tokenOptions = GenerateTokenOptions(signingCredentials, await claims);
                var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                AppUserAuth authUsr = new AppUserAuth()
                {
                    UserName = user.UserName,
                    IsAuthenticated = true,
                    BearerToken = token,
                    UserId = user.Id
                };

                return Ok(authUsr);
            }
            return Unauthorized("Invalid Authentication");

        }

        #region Validate User

        private SigningCredentials GetSigningCredentials()
        {
            var secret = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(jwtSettings.Key));
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings.MinutesToExpiration)),
                signingCredentials: signingCredentials);
            return tokenOptions;
        }

        private async Task<List<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub,
               user.Id.ToString()));
            }
            return claims;
        }
       
        #endregion
    }
}
