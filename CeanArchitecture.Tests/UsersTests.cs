using CeanArchitecture.Tests.Controllers;
using Refit;

namespace CeanArchitecture.Tests
{
    public class UsersTests
    {
        private IUserAccountController userController;
        public IUserAccountController _userController
        {
            get
            {
                userController ??= RestService.For<IUserAccountController>("https://localhost:7219/api");
                return userController;
            }
        }
        [Fact]
        public async Task CheckLogin()
        {
            var token = await _userController.Login(new LoginRequest("sample", "123"));
            var userData = await _userController.ODataTop1("Bearer " + token);
            Assert.True(userData != null && userData.Count > 0);
        }

        [Fact]
        public async Task AuthorizationInvalid()
        {
            await Assert.ThrowsAsync<ApiException>(async () => await _userController.ODataTop1("Bearer "));
        }
        [Fact]
        public async Task CreateUser()
        {
            var user = new UserRegisterRequest("sample", "123", Guid.Parse("e26b6816-3ec1-49b9-8294-05e8ac6d4689"));
            var result = await _userController.Create(user);
            Assert.NotNull(result);
        }
    }
}
