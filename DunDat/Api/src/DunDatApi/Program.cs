using System.Reflection;
using DunDatApi.Data.Books;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Azure;
using RendleLabs.OpenApi.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// if (builder.Environment.IsDevelopment())
// {
//     // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//     builder.Services.AddEndpointsApiExplorer();
//     builder.Services.AddSwaggerGen();
// }

builder.Services.AddSingleton<AuthorData>();
builder.Services.AddSingleton<BookData>();

var azureConnectionString = builder.Configuration.GetConnectionString("AzureStorage");

builder.Services.AddAzureClients(azure => { azure.AddTableServiceClient(azureConnectionString); });

builder.Services.AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(o =>
    {
        o.Authority = "https://dotlabs.eu.auth0.com/";
        o.Audience = "https://api.dundat.net";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
    
    app.UseStaticOpenApi(Assembly.GetExecutingAssembly(), "DunDatApi.openapi.yaml",
            new StaticOpenApiOptions
            {
                Version = 1,
                UiPathPrefix = "swagger",
                JsonPath = "swagger/v1/openapi.json",
                YamlPath = "swagger/v1/openapi.yaml",
                Elements =
                {
                    Path = "swagger/v1/docs"
                }
            })
        .AllowAnonymous();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();