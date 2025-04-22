using Refit;

namespace CeanArchitecture.Tests.Controllers
{
    public static class IUserAccountControllerExtensions
    {
        public static async Task<string> Token()
        {
            IUserAccountController controller = RestService.For<IUserAccountController>("https://localhost:7219/api");
            var token = await controller.Login(new LoginRequest("sample", "123"));
            return $"{token}";
        }
    }
}
