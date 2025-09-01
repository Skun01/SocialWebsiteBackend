using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using SocialWebsite.Data;
using SocialWebsite.Extensions;
using SocialWebsite.Shared.Exceptions;
using FluentValidation;
using FluentValidation.AspNetCore;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Data.Repositories;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Services;
using SocialWebsite.Endpoints;
using Microsoft.AspNetCore.Identity;
using SocialWebsite.Entities;
//BUILDER
var builder = WebApplication.CreateBuilder(args);
// Init serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .CreateLogger();
builder.Host.UseSerilog();

// Database context
string connectionString = builder.Configuration["ConnectionStrings:SqlDocker"]!;
builder.Services.AddDbContext<SocialWebsiteContext>(options => options.UseSqlServer(connectionString));

// Repository Register
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register all valiators for DTOs
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Endpoint services register
builder.Services.AddScoped<IUserService, UserService>();

// Password hasher
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo{ Title = "Social Website API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{ }
        }
    });
});

// converter global from enum to string and versal
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// global error handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

//APPLICATION
var app = builder.Build();
// Middleware:
app.UseSeriRequestLog();
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//ENDPOINT
app.MapUserEndpoints("/users");
app.Run();
