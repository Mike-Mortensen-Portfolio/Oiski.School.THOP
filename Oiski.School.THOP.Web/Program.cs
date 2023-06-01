using Oiski.School.THOP.Web.Services;
using Auth0.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<GlobalState>();
builder.Services.AddScoped<HumidexServiceScope>();
builder.Services.AddScoped<PeripheralServiceScope>();
builder.Services.AddHttpClient("THOP_Api", client =>
{
    //  https://nb4pqt6j-7055.euw.devtunnels.ms
    client.BaseAddress = new Uri("https://nb4pqt6j-7055.euw.devtunnels.ms");    //  Api uses a Dev Tunnel
});
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Security:Domain"] ?? throw new NullReferenceException("Domain secret is null");
    options.ClientId = builder.Configuration["Security:ClientId"] ?? throw new NullReferenceException("Domain secret is null");
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

app.UseCookiePolicy(new CookiePolicyOptions() { MinimumSameSitePolicy = SameSiteMode.None });

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
