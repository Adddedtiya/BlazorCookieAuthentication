using BlazorCookieAuthentication.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

// Authentication Core
builder.Services.AddAuthorizationCore();

// Authentication Service
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthentication>();

// Database layer
builder.Services.AddSingleton<IDatabaseInterface, FakeDatabase>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// For authentication
app.UseAuthentication();
app.UseAuthorization();

app.Run();
