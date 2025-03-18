using Azure.Core;
using Azure;
using CleanBase;
using CleanBase.Entities;
using CleanBusiness;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using System.Text.Json;
using CleanBase.CleanAbstractions.CleanOperation;
using CleanOperation.DataAccess;
using Akka.Actor;
using CleanBase.Dtos;
using static Dapper.SqlMapper;

namespace CleanAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        IConfiguration _configuration;
        public WeatherForecastController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;



        [HttpGet(Name = "GetWeatherForecast")]
        public IActionResult Get()
        {
            TodoList item = new TodoList();
            item.Title = "test";
            item.DueDate = DateTime.Now;
            return Ok(item);
        }
        [HttpGet("Config")]
        public IActionResult Config()
        {

            var data = _configuration["App:Title"];
            return Ok(data);
        }

        [HttpGet("[action]")]
        public IActionResult TestGenerator()
        {

            Compilation inputCompilation = CreateCompilation(@"
        namespace MyCode
        {
            public class Program
            {
                public static void Main(string[] args)
                {
                }
            }
        }
        ");
            ActorGenerator serviceGenerator = new ActorGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(serviceGenerator);
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
            return Ok("test");
        }

        private static Compilation CreateCompilation(string source)
           => CSharpCompilation.Create("compilation",
               new[] { CSharpSyntaxTree.ParseText(source) },
               new[] { MetadataReference.CreateFromFile(typeof(RootService<>).GetTypeInfo().Assembly.Location),
                       MetadataReference.CreateFromFile(typeof(EntityRoot).GetTypeInfo().Assembly.Location)},
               new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    }

    public class MyEndpoint : FastEndpoints.EndpointWithoutRequest<TodoList>
    {
        public override void Configure()
        {
            Get("/api/user/create");
            AllowAnonymous();
            ResponseCache(60);
            SerializerContext<AppJsonSerializerContext>();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            await SendAsync(new TodoList()
            {
                Title = "Test",
                DueDate = DateTime.Now,
            });
        }
    }
    public class TodoListCreate : FastEndpoints.Endpoint<TodoList, EntityResult<TodoList>>
    {
        public IRepository<TodoList> _repository { get; set; }
        public IActorRef _actorRef { get; set; }
        public override void Configure()
        {
            Post("/api/TodoList/Create");
            AllowAnonymous();
            SerializerContext<AppJsonSerializerContext>();
        }

        public override async Task HandleAsync(TodoList req,CancellationToken ct)
        {
            var result = await _actorRef.Ask<EntityResult<TodoList>>(new EntityCommand<TodoList, Guid>
            {
                Entity = req,
                Action = ActionType.Insert
            });
            await SendAsync(result);
        }
    }
    public class TodoListListCreate : FastEndpoints.Endpoint<List<TodoList>, EntityResult<List<TodoList>>>
    {
        public IRepository<TodoList> _repository { get; set; }
        public IActorRef _actorRef { get; set; }
        public override void Configure()
        {
            Post("/api/TodoList/CreateList");
            AllowAnonymous();
            SerializerContext<AppJsonSerializerContext>();
        }

        public override async Task HandleAsync(List<TodoList> req,CancellationToken ct)
        {
            var result = await _actorRef.Ask<EntityResult<List<TodoList>>>(new EntityCommand<TodoList, Guid>
            {
                Entities = req,
                Action = ActionType.InsertList
            });
            await SendAsync(result);
        }
    }
}
