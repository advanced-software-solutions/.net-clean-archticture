using CleanBase.Entities;
using Refit;

namespace CeanArchitecture.Tests
{
    public class UsersTests
    {
        public IUserController userController;
        public IUserController _userController
        {
            get
            {
                userController ??= RestService.For<IUserController>("https://localhost:7219/api");
                return userController;
            }
        }
        [Fact]
        public async Task CheckLogin()
        {
            var token = await _userController.Login(new LoginRequest("amr@gmail.com", "123"));
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
            var user = new UserAccount { Email = "sample", UserRoleId = Guid.Parse("e26b6816-3ec1-49b9-8294-05e8ac6d4689"),
            Password = "test"};
            var result = await _userController.Create(user);
            if (result != null) { }
        }
    }
}
