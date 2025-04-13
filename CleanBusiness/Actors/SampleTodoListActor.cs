using Akka.Actor;
using Akka.Persistence;
using CleanBase.Dtos;
using CleanBase.Entities;
using CleanOperation;
using Microsoft.Extensions.DependencyInjection;

namespace CleanBusiness.Actors;

    public class SampleTodoListActor : UntypedPersistentActor
    {
    private readonly IServiceProvider _sp;
    private readonly IServiceScope _scope;
    private readonly IRepository<TodoList> _repository;
    public SampleTodoListActor(IServiceProvider sp)
        {
        _scope = sp.CreateScope();
        _sp = sp;
        _repository =  _scope.ServiceProvider.GetRequiredService<IRepository<TodoList>>();
        }

    public override string PersistenceId { get; }
        protected override void OnCommand(object message)
        {
            if (message is EntityCommand<TodoList, Guid>)
            {
                var action = message as EntityCommand<TodoList, Guid>;
            try
                {
                Persist(action, evt =>
                {
                    switch (action.Action)
                    {
                        case ActionType.Insert:
                            _repository.Insert(evt.Entity);
                            break;
                        case ActionType.InsertList:
                            break;
                        case ActionType.Update:
                            break;
                        case ActionType.Delete:
                            break;
                        case ActionType.GetById:
                            break;
                        case ActionType.GetPaginated:
                            break;
                        case ActionType.GetByIdCache:
                            break;
                    }
                });
               
            }
            catch (Exception e)
            {
                
            }
            
            }
        }

        protected override void OnRecover(object message)
        {
        throw new NotImplementedException();
    }
}
