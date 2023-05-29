using CleanBase.CleanAbstractions.CleanBusiness;
using Microsoft.AspNetCore.Mvc;

namespace CleanAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        IConfiguration _configuration;
        public WeatherForecastController(ITodoListService todoListService, IConfiguration configuration)
        {
            this.todoListService = todoListService;
            _configuration = configuration;
        }
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ITodoListService todoListService;


        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            todoListService.Insert(new CleanBase.Entities.TodoList { DueDate = DateTime.Now, Title = "Test item" });
            var test = todoListService.Get(y => y.Id > 0);
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
    }
}