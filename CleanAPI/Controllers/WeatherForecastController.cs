using Akka.Actor;
using CleanBase.CleanAbstractions.CleanOperation;
using CleanBase.Dtos;
using CleanBase;
using CleanBase.Entities;
using Enyim.Caching;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CleanBusiness.Actors;

namespace CleanAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        IConfiguration _configuration;
        IUntypedActorContext _actorRef;
        public WeatherForecastController(IConfiguration configuration, IUntypedActorContext actorRef)
        {
            _configuration = configuration;
            _actorRef = actorRef;
        }
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;



        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get()
        {
            var result = await _actorRef.ActorOf<SampleTodoListActor>().Ask<EntityCommand<TodoList, Guid>>(new EntityCommand<TodoList, Guid> { });
            TodoList item = new TodoList();
            item.Title = "test";
            item.DueDate = DateTime.Now;
            return Ok(result);
        }
        [HttpGet("Config")]
        public IActionResult Config()
        {

            var data = _configuration["App:Title"];
            return Ok(data);
        }




    }
}
