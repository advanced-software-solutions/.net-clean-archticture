using Bogus;
using CeanArchitecture.Tests.Controllers;
using Refit;

namespace CeanArchitecture.Tests
{
    public class UserRolesTests
    {
        private IUserRoleController controller;
        public IUserRoleController _controller
        {
            get
            {
                controller ??= RestService.For<IUserRoleController>("https://localhost:7219/api");
                return controller;
            }
        }

        [Fact]
        public async Task Add()
        {
            var role = await _controller.Create(new CleanBase.Entities.UserRole { Name = "Test" });
            Assert.NotNull(role);
        }
        [Fact]
        public async Task CheckAdminRole()
        {
            var role = await _controller.OData(await IUserAccountControllerExtensions.Token(), new ODataQuery(1));
            Assert.NotNull(role);
        }

        [Fact]
        public async Task Update()
        {
            var token = await IUserAccountControllerExtensions.Token();
            var roleList = await _controller.OData(token, new ODataQuery(1));
            var role = roleList.First();
            var newName = new Faker().Random.Word();
            role.Name = newName;
            await _controller.Update(token, role);
            var roleUpdateList = await _controller.OData(token, new ODataQuery(null, $"id eq {role.Id}"));
            Assert.True(roleUpdateList.First().Name == newName);
        }

        [Fact]
        public async Task Delete()
        {
            var token = await IUserAccountControllerExtensions.Token();
            var roleList = await _controller.OData(token, new ODataQuery(1));
            var role = roleList.First();
            await _controller.Delete(role.Id.ToString());
            var deletedRole = await _controller.OData(token,new ODataQuery(null, $"id eq {role.Id}"));
            Assert.True(deletedRole.Count == 0);
        }
    }
}
