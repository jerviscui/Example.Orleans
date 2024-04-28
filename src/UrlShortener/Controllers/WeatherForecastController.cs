using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace UrlShortener.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        private readonly IGrainFactory _grainFactory;

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IGrainFactory grainFactory)
        {
            _logger = logger;
            _grainFactory = grainFactory;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get([FromServices] IConnectionMultiplexer connection)
        {
            //var value = connection.GetDatabase().StringGetSet("test", "value");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();
        }

        [HttpGet("shorten")]
        public async Task<IActionResult> Shorten(string url)
        {
            if (string.IsNullOrWhiteSpace(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                return BadRequest();
            }

            var shortUrl = Guid.NewGuid().GetHashCode().ToString("X");

            var grain = _grainFactory.GetGrain<IUrlShortenerGrain>(shortUrl);
            await grain.SetUrl(url);

            return Ok(shortUrl);
        }

        [HttpGet("getlong")]
        public async Task<IActionResult> Getlong(string shortUrl)
        {
            if (string.IsNullOrWhiteSpace(shortUrl))
            {
                return BadRequest();
            }

            var grain = _grainFactory.GetGrain<IUrlShortenerGrain>(shortUrl);

            var url = await grain.GetUrl();
            if (string.IsNullOrEmpty(url))
            {
                return NotFound();
            }

            return Ok(url);
        }

        [HttpGet("GrainGet")]
        public async Task<IActionResult> GrainGet(string shortUrl)
        {
            var grain = _grainFactory.GetGrain<IUrlGrain>(shortUrl);

            var url = await grain.GetUrl();
            if (string.IsNullOrEmpty(url))
            {
                return NotFound();
            }

            return Ok(url);
        }

        [HttpPost("GrainSet")]
        public async Task<IActionResult> GrainSet()
        {
            var shortUrl = Guid.NewGuid().GetHashCode().ToString("X");

            var grain = _grainFactory.GetGrain<IUrlGrain>(shortUrl);

            await grain.SetUrl(shortUrl);

            return Ok();
        }
    }
}
