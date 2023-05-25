using System.Net.Http.Headers;
using Auth0.AspNetCore.Authentication;
using DunDat.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5003");
});

builder.Services.AddScoped<AccessTokenProvider>();

var auth0Domain = builder.Configuration["Auth0:Domain"];
var auth0ClientId = builder.Configuration["Auth0:ClientId"];
var auth0ClientSecret = builder.Configuration["Auth0:ClientSecret"];

builder.Services.AddAuth0WebAppAuthentication(o =>
    {
        o.Domain = auth0Domain;
        o.ClientId = auth0ClientId;
        o.ClientSecret = auth0ClientSecret;
    })
    .WithAccessToken(o =>
    {
        o.Audience = "https://api.dundat.net";
        o.UseRefreshTokens = true;
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

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();