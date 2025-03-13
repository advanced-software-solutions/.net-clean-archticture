using Akka.Actor;
using CleanBase.Dtos;
using Serilog;

namespace CleanBusiness.Actors;

public class LoggingActor : ReceiveActor
{
    public LoggingActor()
    {
        using var log = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} (CorrelationId:{CorrelationId})] {Message:lj}{NewLine}{Exception}")
            .Enrich.FromLogContext()
            .Enrich.WithProperty("CorrelationId", Guid.NewGuid())
            .CreateLogger();
        Receive<ExecutionUnit>(unit =>
        {
            log.Information("Execution Unit: {@0}", unit);
        });
    }
}
