using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Oiski.School.THOP.Api.Services.DataContainers;
using Oiski.School.THOP.Api.Services.Influx;
using Oiski.School.THOP.Api.Services.MQTT;

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
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(options =>
{
    options.AllowAnyOrigin();
});

app.MapGet("thop/humidex", async ([AsParameters] HumidexFilter filter, InfluxService service) =>
{
    if (filter.StartTime != null && filter.EndTime != null && filter.StartTime > filter.EndTime)
        return Results.BadRequest(new { Error = "Start time can't be higher than end time" });

    var data = await Task.FromResult(
     service.Read<HumidexDTO>()
    .Where(humidex => filter.Sensor == null || humidex.Sensor == filter.Sensor)
    .Where(humidex => filter.LocationId == null || humidex.LocationId == filter.LocationId)
    .Where(humidex => (filter.StartTime == null || humidex.Time!.Value.ToUniversalTime() >= filter.StartTime.Value.ToUniversalTime()) && (filter.EndTime == null || humidex.Time!.Value.ToUniversalTime() <= filter.EndTime.Value.ToUniversalTime()))
    .OrderByDescending(humidex => humidex.Time)
    .ToList());

    //var data = new List<HumidexDTO>
    //{
    //    new HumidexDTO
    //    {
    //        Humidity = 15.2,
    //        LocationId = "home",
    //        Sensor = "DHT11",
    //        Temperature = 22.5,
    //        Time = DateTime.Parse ("2023-05-22T19:09:39.634Z")
    //    },
    //    new HumidexDTO
    //    {
    //        Humidity = 50.2,
    //        LocationId = "home",
    //        Sensor = "DHT11",
    //        Temperature = 40.5,
    //        Time = DateTime.Parse ("2023-05-22T19:09:41.759Z")
    //    },
    //    new HumidexDTO
    //    {
    //        Humidity = 44.2,
    //        LocationId = "home",
    //        Sensor = "DHT11",
    //        Temperature = 12.5,
    //        Time = DateTime.Parse ("2023-05-22T18:12:21.171Z")
    //    }
    //};

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