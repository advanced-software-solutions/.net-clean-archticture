using Akka.Actor;
using Serilog;

namespace CleanBusiness.Actors;

public class SampleTodoListActor : UntypedActor, ICleanActor
{
    protected override void PreStart()
    {
        Log.Information($"Actor {nameof(SampleTodoListActor)} Started");
    }
    protected override void OnReceive(object message)
    {
        switch (message)
        {
            case "test":
                Log.Information("it worked");
                Context.Sender.Tell("tell worked");
                break;
            default:
                break;
        }
    }
}
