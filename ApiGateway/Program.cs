using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Kubernetes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var baseAuthenticationProviderKey = "Auth0";
string authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
string audience = builder.Configuration["Auth0:Audience"] ?? string.Empty;

builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(baseAuthenticationProviderKey, options =>
        {
            options.Authority = authority;
            options.Audience = audience;

        });

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath).AddJsonFile("ocelot.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();
builder.Services.AddOcelot(builder.Configuration).AddCacheManager(x => x.WithDictionaryHandle()).AddKubernetes();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

await app.UseOcelot();

app.UseHttpsRedirection();

app.Run();
