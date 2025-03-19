using Akka.Actor;
using CleanBase.CleanAbstractions.CleanOperation;
using CleanBase.Dtos;
using CleanBase;
using CleanBase.Entities;
using Enyim.Caching;
using Microsoft.AspNetCore.Mvc;

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




    }
}
