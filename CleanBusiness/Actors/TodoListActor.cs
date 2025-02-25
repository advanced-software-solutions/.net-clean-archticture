using Akka.Actor;
using CleanBase.CleanAbstractions.CleanOperation;
using CleanBase.Dtos;
using CleanBase.Entities;
using CleanOperation.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CleanBusiness.Actors
{
    public class TodoListActor : ReceiveActor, ICleanActor
    {

        public TodoListActor(IServiceProvider serviceProvider)
        {
            Receive<EntityCommand<List<TodoList>>>(async msg =>
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var repo = scope.ServiceProvider.GetRequiredService<IRepository<TodoList>>();
                    Context.Self.Tell(await repo.Query().ToListAsync());

                }
            });
        }
    }
}
