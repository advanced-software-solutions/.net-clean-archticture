using Akka.Actor;
using CleanBase.CleanAbstractions.CleanOperation;
using CleanBase.Dtos;
using CleanBase.Entities;
using CleanOperation.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;

namespace CleanBusiness.Actors
{
    public class TodoListActor : ReceiveActor, ICleanActor
    {

        public TodoListActor(IServiceProvider serviceProvider)
        {
            Receive<EntityCommand>(async msg =>
            {
                ExecutionUnit unit = new ExecutionUnit();
                unit.StartTime = DateTime.Now;
                unit.Target = Target.Database;
                unit.Location = $"{nameof(TodoListActor)}:Receive:{nameof(TodoListActor)}";
                unit.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                unit.Name = Assembly.GetExecutingAssembly().GetName().Name;
                unit.EndTime = DateTime.Now;
                unit.Status = CleanBase.Dtos.Status.Success;
                Context.ActorOf<LoggingActor>().Tell(unit);
                using (var scope = serviceProvider.CreateScope())
                {
                    var repo = scope.ServiceProvider.GetRequiredService<IRepository<TodoList>>();
                    Context.Sender.Tell(await repo.Query().ToListAsync());
                    //Sender.Tell(await repo.Query().ToListAsync());

                }
            });

            //Receive<string>(async msg => {
            //    Sender.Tell(msg);
            //});
        }
        protected override void PreStart()
        {
            Debug.WriteLine(Self.Path);
        }
    }

}
