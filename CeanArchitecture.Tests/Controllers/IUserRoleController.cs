using CleanBase.Entities;
using Refit;

namespace CeanArchitecture.Tests.Controllers
{
    public interface IUserRoleController
    {
        private const string controller = nameof(UserRole);
        [Post($"/{controller}")]
        Task<UserRole> Create(UserRole userRole);

        [Get($"/{controller}")]
        Task<List<UserRole>> OData([Authorize] string authorization,[Query]ODataQuery query);

        [Put($"/{controller}")]
        Task<UserRole> Update([Authorize] string authorization, [Body]UserRole userRole);

        [Delete($"/{controller}/{{id}}")]
        Task Delete(string id);
    }

    public record class ODataQuery([AliasAs(@"$top")]int? top, [AliasAs("filter")]string? filter = null);
}
