using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using SocialWebsite.Data;
using SocialWebsite.Extensions;
using SocialWebsite.Shared.Exceptions;
using FluentValidation;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Data.Repositories;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Services;
using SocialWebsite.Endpoints;
using Microsoft.AspNetCore.Identity;
using SocialWebsite.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPostFileRepository, PostFileRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();


// Register all valiators for DTOs
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Endpoint services register
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPostService, PostService>();
// Support services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IFileService, LocalFileService>();
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
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

// Authentication and Authorization
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();

//APPLICATION
var app = builder.Build();
// Middleware:
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseSeriRequestLog();
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//ENDPOINT
var version1 = app.MapGroup("v1");
version1.MapUserEndpoints("/users");
version1.MapAuthEndpoints("/auth");
version1.MapPostEndpoints("/posts");
app.Run();
