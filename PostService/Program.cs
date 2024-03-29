using Microsoft.EntityFrameworkCore;
using PostService.AsyncDataServices;
using PostService.Data;
using PostService.EventProcessing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PostContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostsConn")));

builder.Services.AddScoped<IPostRepo, PostRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessageBusSubscriber>();

var app = builder.Build();
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