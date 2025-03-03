using Akka.Actor;
using CleanBase.CleanAbstractions.CleanOperation;
using CleanBase.Dtos;
using CleanBase.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace CleanAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TodoListActorController(IActorRef actorRef) : ControllerBase
    {
        [HttpGet]
        [EnableQuery]
        public IActionResult Get([FromServices] IRepository<TodoList> repository)
        {
            return Ok(repository.Query());
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TodoList entity)
        {
            var result = await actorRef.Ask<EntityResult<TodoList>>(new EntityCommand<TodoList, Guid>
            {
                Entity = entity,
                Action = ActionType.Insert
            });
            return Ok(result);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> InsertList([FromBody] List<TodoList> entity)
        {
            var result = await actorRef.Ask<EntityResult<List<TodoList>>>(new EntityCommand<TodoList, Guid>
            {
                Entities = entity,
                Action = ActionType.InsertList
            });
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] TodoList entity)
        {
            var result = await actorRef.Ask<TodoList>(new EntityCommand<TodoList, Guid>
            {
                Entity = entity,
                Action = ActionType.Update
            });
            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await actorRef.Ask<EntityResult<TodoList>>(new EntityCommand<TodoList, Guid>
            {
                Id = id,
                Action = ActionType.Delete
            });
            return Ok(result);
        }
    }
}
