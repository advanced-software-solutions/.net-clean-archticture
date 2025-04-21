using CleanBase.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CleanBase.Configurations;
using CleanOperation;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CleanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class UserAccountController : AppBaseController<UserAccount>
    {
        [AllowAnonymous]
        [HttpPost("[action]")]
        public ActionResult<string> Login([FromBody] UserLoginRequest request,
            [FromServices] IConfiguration configuration, [FromServices] IRepository<UserAccount> repository)
        {
            var user = repository.Query().Any(r => r.Email.Equals(request.email)
            && EF.Property<string>(r, nameof(UserAccount.Password)) == request.password);
            if (!user)
            {
                Log.Information("User not found: {@0}", request);
                return BadRequest();
            }
            return Ok(generateJwt(configuration, repository, request));
        }

        private string generateJwt(IConfiguration configuration, IRepository<UserAccount> repository, UserLoginRequest request)
        {
            var userData = repository.Get(y => y.Email.Equals(request.email));
            CleanAppConfiguration appConfig = new();
            configuration.GetSection("CleanAppConfiguration").Bind(appConfig);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfig.Auth.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //If you've had the login module, you can also use the real user information here
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userData.Email),
                new Claim(JwtRegisteredClaimNames.Email, userData.Email),
                new Claim("role", userData.UserRoleId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(appConfig.Auth.Authority,
                appConfig.Auth.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult<UserAccount>> Register([FromBody] UserRegisterRequest registerRequest,
            [FromServices] IRepository<UserAccount> repository)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user =await repository.InsertAsync(
                new UserAccount
                {
                    Email = registerRequest.Email,
                    Password = registerRequest.Password,
                    UserRoleId = registerRequest.UserRoleId
                });
            return Ok(user);
        }
    }



    public record UserLoginRequest(string email, string password)
    {

    }

    public record UserRegisterRequest(string Email, string Password, Guid UserRoleId);
}
