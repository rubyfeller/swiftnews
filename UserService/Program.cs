using Microsoft.EntityFrameworkCore;
using UserService.AsyncDataServices;
using UserService.Auth0;
using UserService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddDbContext<UserContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAuth0Service, Auth0Service>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000", "https://swiftnews.rubyfeller.nl")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

var app = builder.Build();
app.UseCors("AllowLocalhost3000");

await using var scope = app.Services.CreateAsyncScope();
var db = scope.ServiceProvider.GetService<UserContext>();
if (db != null)
{
    await db.Database.MigrateAsync();
}
else
{
    Console.WriteLine("Could not migrate database");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
