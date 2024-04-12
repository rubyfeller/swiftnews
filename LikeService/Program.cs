using LikeService.AsyncDataServices;
using LikeService.Clients;
using LikeService.Data;
using LikeService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var factory = new ConnectionFactory() { HostName = builder.Configuration["RabbitMQHost"] };
builder.Services.AddSingleton<IConnection>(sp => factory.CreateConnection());

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.Configure<LikeStoreDatabaseSettings>(builder.Configuration.GetSection(nameof(LikeStoreDatabaseSettings)));

builder.Services.AddSingleton<ILikeStoreDatabaseSettings>(sp => sp.GetRequiredService<IOptions<LikeStoreDatabaseSettings>>().Value);

builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(builder.Configuration.GetValue<string>("LikeStoreDatabaseSettings:ConnectionString")));

builder.Services.AddScoped<ILikeRepository, LikeRepository>();

builder.Services.AddHttpClient<IPostServiceClient, PostServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("PostServiceSettings:BaseUrl"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();