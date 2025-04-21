using CleanBase.Entities;
using Refit;

namespace CeanArchitecture.Tests.Controllers
{
    public interface IUserAccountController
    {
        [Get("/UserAccount?$top=1")]
        Task<List<UserAccount>> ODataTop1([Header("Authorization")] string authorization);

        [Post("/UserAccount/Login")]
        Task<string> Login([Body] LoginRequest request);
        [Post("/UserAccount/Register")]
        Task<UserAccount> Create([Body] UserRegisterRequest request);
    }

    public record LoginRequest(string email, string password);
    public record UserRegisterRequest(string Email, string Password, Guid UserRoleId);
}
