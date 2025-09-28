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
using SocialWebsite.Hubs;
using SocialWebsite.Authorization;
using Microsoft.AspNetCore.Authorization;
using SocialWebsite.Swagger;
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

builder.Services.AddCors(option =>
{
    option.AddPolicy("react",
    p => p.WithOrigins("http://localhost:5173", "http://localhost:3000")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials()
    );
});

// SignalR service
builder.Services.AddSignalR();

// IMemoryCache service
builder.Services.AddMemoryCache();

// Repository Register
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPostFileRepository, PostFileRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IFriendshipRepository, FriendshipRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddSingleton<ICacheService, LocalCacheService>();


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
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IFriendshipService, FriendshipService>();
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
    // Hiển thị yêu cầu Authorize/Role
    options.OperationFilter<AuthorizationRequirementsOperationFilter>();
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
// Authorization với role-based policies
builder.Services.AddAuthorization(options =>
{
    // Tạo policies cho từng role
    options.AddPolicy("RequireRole_User", policy => 
        policy.Requirements.Add(new RoleRequirement(SocialWebsite.Shared.Enums.UserRole.User)));
    options.AddPolicy("RequireRole_Moderator", policy => 
        policy.Requirements.Add(new RoleRequirement(SocialWebsite.Shared.Enums.UserRole.Moderator)));
    options.AddPolicy("RequireRole_Admin", policy => 
        policy.Requirements.Add(new RoleRequirement(SocialWebsite.Shared.Enums.UserRole.Admin)));
});

// Đăng ký authorization handler
builder.Services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();

//APPLICATION
var app = builder.Build();
// Middleware:
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseSeriRequestLog();
app.UseExceptionHandler();
app.UseCors("react");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<ChatHub>("/chatHub");
app.MapHub<NotificationHub>("/notificationHub");

//ENDPOINT
var version1 = app.MapGroup("v1");
version1.MapUserEndpoints("/users");
version1.MapAuthEndpoints("/auth");
version1.MapPostEndpoints("/posts");
version1.MapCommentEndpoints("/comments");
version1.MapChatEndpoints("/chat");
version1.MapNotificationEndpoints("/notifications");
version1.MapFriendShipEndpoints("/friendships");
version1.MapAdminEndpoints("/admin");
app.Run();
