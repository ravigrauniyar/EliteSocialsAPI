using Application.UserService.Interfaces;
using Application.UserService.Services;
using EliteSocialsAPI.Controllers;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the DI container.
builder.Services.AddControllers().AddJsonOptions(
    options =>
    {
        // Don't apply default camelCaseNamingPolicy to names of Properties of instances
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        builder =>
        {
            // Allow consume's host to access the API
            builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowedToAllowWildcardSubdomains();
        });
});

// Add Database connection
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DbConnection")
    )
);

// Add services for Swagger
builder.Services.AddSwaggerGen(c =>
{
    // Documentation of Swagger application
    c.SwaggerDoc("ES_Swagger", new OpenApiInfo
    {
        Title = "EliteSocials application",
    });
    // Adds scheme required for Authorization
    c.AddSecurityDefinition("ES JWT Authentication",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        }
    );
    // Adds Authorization requirement for Controllers/Methods
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ES JWT Authentication"
                }
            },
            Array.Empty<string>()
        }
    });
});
// Add Services required for Authentication
builder.Services.AddAuthentication("Bearer").
    // Enables JWT Authentication using Bearer scheme
    AddJwtBearer(options =>
    {
        // Set parameters for validating tokens
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:issuer"],
            ValidAudience = builder.Configuration["Jwt:audience"],

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]!))
        };
    }
);

builder.Services.AddScoped<BaseController>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Adds SwaggerUI Middleware
    app.UseSwaggerUI(
            c =>
            {
                c.SwaggerEndpoint("/swagger/ES_Swagger/swagger.json", "ES Swagger Endpoint");
            }
        );
    // Adds Swagger Middleware
    app.UseSwagger();
}

// Adds Swagger Middleware
app.UseSwagger();

// Adds Middleware to redirect HTTP requests to HTTPS
app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

// Adds Authentication Middleware
app.UseAuthentication();
// Adds Authorization Middleware
app.UseAuthorization();
// Maps Controllers with RouteBuilder
app.MapControllers();

app.Run();
