using LikeService.AsyncDataServices;
using LikeService.Data;
using LikeService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.Configure<LikeStoreDatabaseSettings>(builder.Configuration.GetSection(nameof(LikeStoreDatabaseSettings)));

builder.Services.AddSingleton<ILikeStoreDatabaseSettings>(sp => sp.GetRequiredService<IOptions<LikeStoreDatabaseSettings>>().Value);

builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(builder.Configuration.GetValue<string>("LikeStoreDatabaseSettings:ConnectionString")));

Console.WriteLine(builder.Configuration.GetValue<string>("LikeStoreDatabaseSettings:ConnectionString"));

builder.Services.AddScoped<ILikeRepository, LikeRepository>();

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