using CleanBase.Entities;
using Refit;

namespace CeanArchitecture.Tests
{
    public interface IUserController
    {
        [Get("/UserAccount?$top=1")]
        Task<List<UserAccount>> ODataTop1([Header("Authorization")] string authorization);

        [Post("/UserAccount/Login")]
        Task<string> Login([Body] LoginRequest request); 
        [Post("/UserAccount/Register")]
        Task<string> Create([Body] UserAccount request);
    }

    public record LoginRequest(string username,string password);
}
