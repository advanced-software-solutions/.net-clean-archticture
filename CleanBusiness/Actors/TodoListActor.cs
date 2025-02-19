using Akka.Actor;
using CleanBase.CleanAbstractions.CleanOperation;
using CleanBase.Dtos;
using CleanBase.Entities;
using CleanOperation.Abstractions;

namespace CleanBusiness.Actors
{
    public class TodoListActor : ReceiveActor, ICleanActor
    {
        public TodoListActor(IRepository<TodoList> repository)
        {
            Receive<EntityCommand<TodoListActor>>(async msg =>
            {
                    
            });
        }
    }
}
