using CleanBase.CleanAbstractions.CleanOperation;
using CleanBase.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CleanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class UserAccountController : AppBaseController<UserAccount>
    {
        [AllowAnonymous]
        [HttpPost("[action]")]
        public ActionResult<string> Login([FromBody]UserLoginRequest request,
            [FromServices] IConfiguration configuration, [FromServices] IRepository<UserAccount> repository)
        {
            var user = repository.Query().Any(r => r.Email.Equals(request.userName)
            && r.Password.Equals(request.password));
            if (!user) return BadRequest();
            return Ok(generateJwt(configuration, repository, request));
        }

        private string generateJwt(IConfiguration configuration, IRepository<UserAccount> repository, UserLoginRequest request)
        {
            var userData = repository.Get(y => y.Email.Equals(request.userName));
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Auth:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //If you've had the login module, you can also use the real user information here
            var claims = new[] {
        new Claim(JwtRegisteredClaimNames.Sub, userData.Email),
        new Claim(JwtRegisteredClaimNames.Email, userData.Email),
        new Claim("role", userData.UserRoleId.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var token = new JwtSecurityToken(configuration["Auth:Authority"],
                configuration["Auth:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }


    public record UserLoginRequest(string  userName, string password)
    {

    }
}
