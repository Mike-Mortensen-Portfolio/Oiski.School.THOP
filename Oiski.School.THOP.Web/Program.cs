using Oiski.School.THOP.Web.Services;
#if USE_AUTH0
using Auth0.AspNetCore.Authentication;
#endif
using Oiski.School.THOP.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<GlobalState>();
builder.Services.AddScoped<HumidexServiceScope>();
builder.Services.AddScoped<PeripheralServiceScope>();
builder.Services.AddHttpClient("THOP_Api", client =>
{
    client.BaseAddress = new Uri("http://10.135.16.57:8080/");    //  Api uses a Dev Tunnel
});
#if USE_AUTH0
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Security:Domain"] ?? throw new NullReferenceException("Domain secret is null");
    options.ClientId = builder.Configuration["Security:ClientId"] ?? throw new NullReferenceException("Client secret is null");
});
#endif

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

#if USE_AUTH0
app.UseCookiePolicy(new CookiePolicyOptions() { MinimumSameSitePolicy = SameSiteMode.None });

app.UseAuthentication();
app.UseAuthorization();
#endif

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
