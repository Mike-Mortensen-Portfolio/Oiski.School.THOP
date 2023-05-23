using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Oiski.School.THOP.Api;
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
    if (ManualError.ErrorThrow)
        return Results.Problem(detail: "This is a test throw!", statusCode: StatusCodes.Status500InternalServerError);

    if (filter.StartTime != null && filter.EndTime != null && filter.StartTime > filter.EndTime)
        return Results.BadRequest(new { Error = "Start time can't be higher than end time" });

    #region Seed
    //IEnumerable<HumidexDTO> data = new List<HumidexDTO>
    //{
    //    new HumidexDTO
    //    {
    //        Humidity = 60.2,
    //        LocationId = "home",
    //        Sensor = "DHT11",
    //        Temperature = 22.5,
    //        Time = DateTime.Parse ("2023-05-19T16:00:00.0004Z")
    //    },
    //    new HumidexDTO
    //    {
    //        Humidity = 67,
    //        LocationId = "home",
    //        Sensor = "DHT11",
    //        Temperature = 25.5,
    //        Time = DateTime.Parse ("2023-05-22T17:00:00.000Z")
    //    },
    //    new HumidexDTO
    //    {
    //        Humidity = 50.2,
    //        LocationId = "home",
    //        Sensor = "DHT11",
    //        Temperature = 19.5,
    //        Time = DateTime.Parse ("2023-05-22T18:00:00.000Z")
    //    }
    //}
    #endregion

    IEnumerable<HumidexDTO> data = await Task.FromResult(
     service.Read<HumidexDTO>()
    .Where(humidex => string.IsNullOrWhiteSpace(filter.Sensor) || humidex.Sensor == filter.Sensor)
    .Where(humidex => string.IsNullOrWhiteSpace(filter.LocationId) || humidex.LocationId == filter.LocationId)
    .Where(humidex => (filter.StartTime == null || humidex.Time!.Value.ToUniversalTime() >= filter.StartTime.Value.ToUniversalTime()) && (filter.EndTime == null || humidex.Time!.Value.ToUniversalTime() <= filter.EndTime.Value.ToUniversalTime()))
    .OrderByDescending(humidex => humidex.Time));

    if (filter.MaxCount.HasValue)
        data = data.Take(filter.MaxCount.Value);

    return Results.Ok(data.ToList());
});

app.MapPost("thop/ventilation", async ([FromBody] VentilationOptions options, MyMQTTClient client) =>
{
    var topic = $"{options.LocationId.ToLowerInvariant()}/{options.DeviceId.ToLowerInvariant()}";
    var result = await client.PubAsync(topic,
        JsonConvert.SerializeObject(new
        {
            options.LocationId,
            options.DeviceId,
            Vents = ((options.Open) ? ("On") : ("Off"))
        }));

    if (!result.IsSuccess)
        return Results.Problem($"Error: {result}", statusCode: StatusCodes.Status500InternalServerError);

    return Results.Ok(new
    {
        Topic = topic,
        Vents = ((options.Open) ? ("On") : ("Off")),
        StatusCode = result
    });
});

app.MapGet("thop/killHumidex", () =>
{
    ManualError.ErrorThrow = !ManualError.ErrorThrow;

    return Results.Ok(ManualError.ErrorThrow);
});

app.Run();