using CleanBase;
using CleanBase.CleanAbstractions.CleanOperation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace CleanAPI.Controllers;
//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AppBaseController<TEntity> : ControllerBase where TEntity : class, IEntityRoot
{
    [HttpGet]
    [EnableQuery]
    public IActionResult Get([FromServices] IRepository<TEntity> repository)
    {
        return Ok(repository.Query());
    }
}
