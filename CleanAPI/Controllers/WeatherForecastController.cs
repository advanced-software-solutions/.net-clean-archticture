using CleanBase;
using CleanBase.Entities;
using CleanBusiness;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using System.Text.Json;

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
            var jsonString = JsonSerializer.Serialize(item);
            return Ok(jsonString);
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
}
