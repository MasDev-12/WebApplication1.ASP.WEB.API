using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using WebApplication.Service.Interfaces;
using WebApplication.Domain.Enum;
using WebApplication.Domain.ViewModel.AccountViewModel;

namespace WebApplication1.ASP.WEB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController: Controller
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;
        private object _configuration;

        public AccountController(IAccountService accountService,IUserService userService, IConfiguration configuration)
        {
            _accountService=accountService;
            _userService=userService;
            _configuration=configuration;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _accountService.RegisterUser(model);
            if (response.StatusCode==StatusCodeEnum.Found)
            {
                ModelState.AddModelError("", response.Description);
                return StatusCode(422, ModelState);
            }
            await _userService.CreateUser(response.Data);
            if (response.StatusCode==StatusCodeEnum.OK)
            {
                return Ok("Successfully created");
            }
            return StatusCode(500);
        }



        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var user = await _accountService.LoginUser(model);

            if (user.StatusCode == StatusCodeEnum.OK)
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Data.Name),
                    new Claim(ClaimTypes.Role, user.Data.Role.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                var config = (IConfiguration)_configuration;
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));

                var token = new JwtSecurityToken(
                    issuer: config["Jwt:Issuer"],
                    audience: config["Jwt:Audience"],
                    expires: DateTime.UtcNow.AddSeconds(3600),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);

                var respone = new { access_token = encodedJwt, username = user.Data.Name };
                return Json(respone);

                //return Ok(new
                //{
                //    token = new JwtSecurityTokenHandler().WriteToken(token),
                //    expiration = token.ValidTo
                //});
            }
            return Unauthorized();

        }

       [HttpGet]
       [Route("getAllUsers")]
       [Authorize(Roles = "Administrator")]
        public IActionResult getAllUsers()
        {
            var users = _userService.GetUsers();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(users.Data);
        }
    }
}
