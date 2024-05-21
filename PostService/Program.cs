using Microsoft.EntityFrameworkCore;
using PostService.AsyncDataServices;
using PostService.Data;
using PostService.EventProcessing;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PostContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostsConn")));

Console.WriteLine("Connection string: " + builder.Configuration.GetConnectionString("PostsConn"));

builder.Services.AddScoped<IPostRepo, PostRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IEventProcessor, EventProcessor>();
builder.Services.AddSingleton<IHostedService, MessageBusSubscriber>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

var app = builder.Build();
app.UseCors("AllowLocalhost3000");

app.UseMetricServer();

await using var scope = app.Services.CreateAsyncScope();
var db = scope.ServiceProvider.GetService<PostContext>();
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

app.MapControllers();

app.Run();