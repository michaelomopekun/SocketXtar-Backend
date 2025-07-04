using MediatR;
// using DotNetEnv
using MongoDB.Driver;
using Persistence.Data;
using Persistence.Data.Mongo;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Helpers;

EnvLoader.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine("========⌚⌚⌚"+Environment.GetEnvironmentVariable("POSTGRES_HOST")+"========⌚⌚⌚");

// Configure PostgresDbContext
var connectionString = $"Server={Environment.GetEnvironmentVariable("POSTGRES_HOST")};" +
                      $"Port={Environment.GetEnvironmentVariable("POSTGRES_PORT")};" +
                      $"Database={Environment.GetEnvironmentVariable("POSTGRES_DB")};" +
                      $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};" +
                      $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")};" +
                      $"SSL Mode={Environment.GetEnvironmentVariable("POSTGRES_SSLMODE")};Trust Server Certificate=true";

builder.Services.AddDbContext<ChatAppDBContext>(options =>
    options.UseNpgsql(connectionString));

//MongoDB configuration
builder.Services.AddSingleton<IMongoClient>(s =>
{
    var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
    if (string.IsNullOrEmpty(connectionString))
        throw new InvalidOperationException("MONGODB_CONNECTION_STRING environment variable is not set.");
    return new MongoClient(connectionString);
});

builder.Services.AddSingleton<ChatDbContext>();

builder.Services.AddMediatR(typeof(Program).Assembly);

// redis config
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = Environment.GetEnvironmentVariable("REDIS");
    options.InstanceName = "XtarSocket:";
});


var app = builder.Build();

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
