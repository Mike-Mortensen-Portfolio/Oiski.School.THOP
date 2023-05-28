using Oiski.School.THOP.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<HumidexServiceScope>();
builder.Services.AddScoped<PeripheralServiceScope>();
builder.Services.AddHttpClient("THOP_Api", client =>
{
    client.BaseAddress = new Uri("https://kvttffdl-7055.euw.devtunnels.ms");    //  Api uses a Dev Tunnel
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
