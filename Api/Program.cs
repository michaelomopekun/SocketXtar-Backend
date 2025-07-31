using MediatR;
// using DotNetEnv
using MongoDB.Driver;
using Persistence.Data;
using Persistence.Data.Mongo;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Helpers;
using Application.Users.Common.Interface;
using Infrastructure.Services;
using Domain.Interfaces.Repositories;
using Infrastructure.Repositories;
using Application.Users.Handlers;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Api.Middlewares;
using CloudinaryDotNet;
using Application.Users.Common.Exceptions;

EnvLoader.Load();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine("========⌚⌚⌚"+Environment.GetEnvironmentVariable("POSTGRES_HOST")+"========⌚⌚⌚");

// Postgres connection string
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

builder.Services.AddMediatR(typeof(RegisterUserHandler).Assembly);


// redis config
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = Environment.GetEnvironmentVariable("REDIS");
    options.InstanceName = "XtarSocket:";
});


// JWT authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new NullReferenceException("env var JWT_SECRET cant be found"));

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = true
        };
    });


// JWT + Swagger configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "SocketXtar API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            securityScheme,
            Array.Empty<string>()
        }
    });
});

// Cloudinary config
builder.Services.AddSingleton(serviceProvider =>
{
    var account = new Account(
        Environment.GetEnvironmentVariable("CLOUDNAME"),
        Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY"),
        Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET"));

    return new Cloudinary(account);
});


// Register services
//services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
//repos
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();



var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseMiddleware<TokenBlacklistMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
