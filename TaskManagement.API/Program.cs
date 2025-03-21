using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManagement.Application;
using TaskManagement.Application.Common.Settings;
using TaskManagement.Application.Common.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.OpenApi.Models;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.API.Services;
using TaskManagement.API.Middleware;
using TaskManagement.Domain.Common.Interfaces;
using MediatR;
using AutoMapper;
using FluentValidation.AspNetCore;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register CurrentUserService
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Add Application Services
builder.Services.AddApplication();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(TaskManagement.Application.DependencyInjection).Assembly));
builder.Services.AddAutoMapper(typeof(TaskManagement.Application.DependencyInjection).Assembly);

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(TaskManagement.Application.DependencyInjection).Assembly);

// Add Infrastructure Services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TaskManagementDb"));

builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

// Configure Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure JWT Settings
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IJwtService, JwtService>();

// Configure Authentication & Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskManagement API", Version = "v1" });

    // Configure JWT Authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManagement API V1");
        c.DocExpansion(DocExpansion.None);
    });
}

app.UseHttpsRedirection();

// Add Session Middleware
app.UseSession();

// Add User Context Middleware
app.UseUserContext();

// Important: UseAuthentication must come before UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

// Initialize Database and Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
        SeedData(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.MapControllers();

app.Run();

static void SeedData(ApplicationDbContext context)
{
    // Add Departments
    if (!context.Departments.Any())
    {
        var departments = new[]
        {
            new Department { Id = Guid.NewGuid(), Name = "IT Department", Description = "IT Department" },
            new Department { Id = Guid.NewGuid(), Name = "HR Department", Description = "HR Department" },
            new Department { Id = Guid.NewGuid(), Name = "Finance Department", Description = "Finance Department" }
        };
        context.Departments.AddRange(departments);
        context.SaveChanges();

        // Add Users
        var users = new[]
        {
            new User { Id = Guid.NewGuid(), Name = "Gökhan Övsene", Email = "gokhan.ovsene@id3.com.tr", DepartmentId = departments[0].Id },
            new User { Id = Guid.NewGuid(), Name = "Nur Uçar", Email = "nur.ucar@id3.com.tr", DepartmentId = departments[1].Id },
            new User { Id = Guid.NewGuid(), Name = "Kenan Eraslan", Email = "kenan.eraslan@id3.com.tr", DepartmentId = departments[2].Id },
            new User { Id = Guid.NewGuid(), Name = "Ertürk Yılmaz", Email = "erturk.yilmaz@id3.com.tr", DepartmentId = departments[0].Id }
        };
        context.Users.AddRange(users);
        context.SaveChanges();

        // Add Tasks
        var tasks = new[]
        {
            new TaskManagement.Domain.Entities.Task
            {
                Id = Guid.NewGuid(),
                Title = "Implement User Authentication",
                Description = "Add JWT authentication to the API",
                CreatedDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7),
                Status = TaskManagement.Domain.Enums.TaskStatus.Created,
                CreatedById = users[0].Id,
                AssignedToId = users[1].Id,
                DepartmentId = departments[1].Id
            },
            new TaskManagement.Domain.Entities.Task
            {
                Id = Guid.NewGuid(),
                Title = "Update Employee Handbook",
                Description = "Review and update the company's employee handbook",
                CreatedDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14),
                Status = TaskManagement.Domain.Enums.TaskStatus.Assigned,
                CreatedById = users[1].Id,
                AssignedToId = users[2].Id,
                DepartmentId = departments[2].Id
            },
            new TaskManagement.Domain.Entities.Task
            {
                Id = Guid.NewGuid(),
                Title = "Prepare Monthly Report",
                Description = "Generate financial report for the current month",
                CreatedDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(5),
                Status = TaskManagement.Domain.Enums.TaskStatus.InProgress,
                CreatedById = users[2].Id,
                AssignedToId = users[0].Id,
                DepartmentId = departments[0].Id
            },
            new TaskManagement.Domain.Entities.Task
            {
                Id = Guid.NewGuid(),
                Title = "Prepare Yearly Report",
                Description = "Generate financial report for the current year",
                CreatedDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(5),
                Status = TaskManagement.Domain.Enums.TaskStatus.InProgress,
                CreatedById = users[0].Id,
                AssignedToId = users[3].Id,
                DepartmentId = departments[0].Id
            }
        };
        context.Tasks.AddRange(tasks);
        context.SaveChanges();
    }
}
