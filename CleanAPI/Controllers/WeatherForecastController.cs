using Akka.Actor;
using CleanBase;
using CleanBase.CleanAbstractions.CleanBusiness;
using CleanBase.Dtos;
using CleanBase.Entities;
using CleanBusiness;
using CleanBusiness.Actors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;
using System.Reflection;

namespace CleanAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    IConfiguration _configuration;
    private readonly IActorRef actorRef;

    public WeatherForecastController(IConfiguration configuration, IActorRef actorRef)
    {
        _configuration = configuration;
        this.actorRef = actorRef;
    }
    private static readonly string[] Summaries = new[]
    {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

    private readonly ILogger<WeatherForecastController> _logger;
    


    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
    [HttpGet("Config")]
    public IActionResult Config()
    {
        var data = _configuration["App:Title"];
        return Ok(data);
    }
    [HttpGet("[action]")]
    public async Task<IActionResult> TestActor()
    {
        //var result = await actorRef.Ask<string>("hello world!");
        return Ok();
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
        ServiceGenerator serviceGenerator = new ServiceGenerator();
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
