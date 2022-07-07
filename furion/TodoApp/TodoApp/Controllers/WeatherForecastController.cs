using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TodoApp.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly MyDbContext _myDbContext;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,MyDbContext myDbContext)
    {
        _logger = logger;
        _myDbContext = myDbContext;
    }

    [HttpGet]
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
    
    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var todoItem = new TodoItem()
        {
            Id = Guid.NewGuid().ToString("n"),
            Text = Guid.NewGuid().ToString("n")
        };
        await _myDbContext.AddAsync(todoItem);
        await _myDbContext.SaveChangesAsync();
        return Ok(todoItem);
    }
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var list = await _myDbContext.Set<TodoItem>().ToListAsync();
        return Ok(list);
    }
}