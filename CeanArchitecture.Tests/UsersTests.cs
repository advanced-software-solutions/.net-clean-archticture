using CeanArchitecture.Tests.Controllers;
using CleanBase;
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
            var token = await _userController.Login(new LoginRequest("admin", "123"));
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
            var user = new UserRegisterRequest("admin", "123", Constants.AdminRoleId);
            var result = await _userController.Create(user);
            Assert.NotNull(result);
        }
    }
}
