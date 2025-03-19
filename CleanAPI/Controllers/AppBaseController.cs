using Akka.Actor;
using CleanBase;
using CleanBase.CleanAbstractions.CleanOperation;
using CleanBase.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace CleanAPI.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AppBaseController<TEntity>(IActorRef actorRef) : ControllerBase where TEntity : class, IEntityRoot
{
    [HttpGet]
    [EnableQuery]
    public IActionResult Get([FromServices] IRepository<TEntity> repository)
    {
        return Ok(repository.Query());
    }
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TEntity entity)
    {
        var result = await actorRef.Ask<EntityResult<TEntity>>(new EntityCommand<TEntity, Guid>
        {
            Entity = entity,
            Action = ActionType.Insert
        });
        return Ok(result);
    }
    [HttpPost("[action]")]
    public async Task<IActionResult> InsertList([FromBody] List<TEntity> entity)
    {
        var result = await actorRef.Ask<EntityResult<List<TEntity>>>(new EntityCommand<TEntity, Guid>
        {
            Entities = entity,
            Action = ActionType.InsertList
        });
        return Ok(result);
    }
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] TEntity entity)
    {
        var result = await actorRef.Ask<TEntity>(new EntityCommand<TEntity, Guid>
        {
            Entity = entity,
            Action = ActionType.Update
        });
        return Ok(result);
    }
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await actorRef.Ask<EntityResult<TEntity>>(new EntityCommand<TEntity, Guid>
        {
            Id = id,
            Action = ActionType.Delete
        });
        return Ok(result);
    }
}
