using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Oiski.School.THOP.Api.Services.DataContainers;
using Oiski.School.THOP.Api.Services.Influx;
using Oiski.School.THOP.Api.Services.MQTT;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<InfluxService>();
builder.Services.AddSingleton((provider) =>
{
    return new MyMQTTClient(Console.WriteLine);
});
builder.HookMQTTWorker();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("thop/humidex", async ([AsParameters] HumidexFilter filter, InfluxService service) =>
{
    if (filter.StartTime != null && filter.EndTime != null && filter.StartTime > filter.EndTime)
        return Results.BadRequest(new { Error = "Start time can't be higher than end time" });

    var data = await Task.FromResult(
     service.Read<HumidexDTO>()
    .Where(humidex => filter.Sensor == null || humidex.Sensor == filter.Sensor)
    .Where(humidex => filter.LocationId == null || humidex.LocationId == filter.LocationId)
    .Where(humidex => (filter.StartTime == null || humidex.Time >= filter.StartTime) && (filter.EndTime == null || humidex.Time <= filter.EndTime))
    .OrderByDescending(humidex => humidex.Time)
    .ToList());

    return Results.Ok(data);
});

app.MapPost("thop/ventilation", async ([FromBody] VentilationOptions options, MyMQTTClient client) =>
{
    var topic = $"{options.LocationId.ToLowerInvariant()}/{options.DeviceId.ToLowerInvariant()}";
    var result = await client.PubAsync(topic,
        JsonConvert.SerializeObject(new
        {
            options.LocationId,
            options.DeviceId,
            Vents = ((options.On) ? ("On") : ("Off"))
        }));

    if (!result.IsSuccess)
        return Results.Problem($"Error: {result}", statusCode: StatusCodes.Status500InternalServerError);

    return Results.Ok(new
    {
        Topic = topic,
        Vents = ((options.On) ? ("On") : ("Off")),
        StatusCode = result
    });
});

app.Run();