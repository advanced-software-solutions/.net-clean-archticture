using Akka.Actor;
using CleanBase.CleanAbstractions.CleanOperation;
using CleanBase.Dtos;
using CleanBase.Entities;
using CleanOperation.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace CleanBusiness.Actors
{
    public partial class TodoListActor : ReceiveActor, ICleanActor
    {

        public TodoListActor(IServiceProvider serviceProvider)
        {
            Receive<EntityCommand<TodoList, Guid>>(async msg =>
            {
                //ExecutionUnit unit = new ExecutionUnit();
                //unit.StartTime = DateTime.Now;
                //unit.Target = Target.Database;
                //unit.Location = $"{nameof(TodoListActor)}:Receive:{nameof(TodoListActor)}";
                //unit.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                //unit.Name = Assembly.GetExecutingAssembly().GetName().Name;
                using (var scope = serviceProvider.CreateScope())
                {
                    var repo = scope.ServiceProvider.GetRequiredService<IRepository<TodoList>>();

                    switch (msg.Action)
                    {
                        case ActionType.Insert:
                            await Insert(repo, msg);
                            break;
                        case ActionType.InsertList:
                            await InsertList(repo, msg);
                            break;
                        case ActionType.Update:
                            Update(repo, msg);
                            break;
                        case ActionType.Delete:
                            Delete(repo, msg);
                            break;
                        case ActionType.GetById:
                            await GetById(repo, msg);
                            break;
                        case ActionType.GetPaginated:
                            break;
                    }
                }
                //unit.Status = CleanBase.Dtos.Status.Success;
                //unit.EndTime = DateTime.Now;
                //Context.ActorOf<LoggingActor>().Tell(unit);
            });
        }
        protected override void PreStart()
        {
            Debug.WriteLine(Self.Path);
        }

        private async Task Insert(IRepository<TodoList> repo,
            EntityCommand<TodoList, Guid> msg)
        {
            EntityResult<TodoList> entityResult = new();
            entityResult.Data = await repo.InsertAsync(msg.Entity);
            entityResult.IsSuccess = true;
            Context.Sender.Tell(entityResult);
        }
        private async Task InsertList(IRepository<TodoList> repo,
            EntityCommand<TodoList, Guid> msg)
        {
            EntityResult<List<TodoList>> entityResult = new();
            await repo.InsertAsync(msg.Entities);
            entityResult.IsSuccess = true;
            Context.Sender.Tell(entityResult);
        }
        private void Update(IRepository<TodoList> repo,
           EntityCommand<TodoList, Guid> msg)
        {
            EntityResult<TodoList> entityResult = new();
            repo.Update(msg.Entity);
            entityResult.IsSuccess = true;
            Context.Sender.Tell(entityResult);
        }
        private void Delete(IRepository<TodoList> repo,
           EntityCommand<TodoList, Guid> msg)
        {
            EntityResult<TodoList> entityResult = new();
            repo.Delete(msg.Id);
            entityResult.IsSuccess = true;
            entityResult.Details =new() { { "Id", msg.Id } };
            Context.Sender.Tell(entityResult);
        }
        private async Task GetById(IRepository<TodoList> repo,
           EntityCommand<TodoList, Guid> msg)
        {
            EntityResult<TodoList> entityResult = new();
            entityResult.Data = await  repo.GetAsync(msg.Id);
            entityResult.IsSuccess = true;
            Context.Sender.Tell(entityResult);
        }
    }

}
