using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly IGrainFactory _grainFactory;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IGrainFactory grainFactory)
        {
            _logger = logger;
            _grainFactory = grainFactory;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
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

            var guid = Guid.NewGuid().GetHashCode().ToString("X");

            var grain = _grainFactory.GetGrain<IUrlShortenerGrain>(guid);
            await grain.SetUrl(url);

            return Ok(guid);
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
    }
}
