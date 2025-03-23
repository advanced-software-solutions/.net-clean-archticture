using Akka.Actor;
using Akka.Persistence;
using CleanBase.Dtos;
using CleanBase.Entities;

namespace CleanBusiness.Actors
{
    public class SampleTodoListActor : UntypedPersistentActor
    {
        public override string PersistenceId  => Guid.NewGuid().ToString();
        protected override void PreStart()
        {
            Serilog.Log.Information($"Starting {nameof(SampleTodoListActor)}");
            base.PreStart();
        }
        protected override void OnCommand(object message)
        {
            if (message is EntityCommand<TodoList, Guid>)
            {
                var action = message as EntityCommand<TodoList, Guid>;
                Persist(action, (evt) =>
                {
                    Context.Sender.Tell("Success");
                });
            }
        }

        protected override void OnRecover(object message)
        {
            
        }
    }
}
