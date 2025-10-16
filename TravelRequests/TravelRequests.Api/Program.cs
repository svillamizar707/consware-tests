using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using TravelRequests.Application.Interfaces;
using TravelRequests.Application.Services;
using TravelRequests.Infrastructure.Data;
using TravelRequests.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using TravelRequests.Api.Middleware;
using TravelRequests.Api.Serilog;
using Serilog;
using TravelRequests.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog early
SerilogExtensions.ConfigureSerilog(builder.Configuration);
builder.Host.UseSerilog();

// Registrar servicios antes de app.Build()
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<TravelRequests.Application.Validators.CreateTravelRequestValidator>();

// Configure Swagger/OpenAPI with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Travel Requests API",
        Version = "v1",
        Description = "API para gestionar solicitudes de viajes corporativos.\n\nAutenticación: use JWT Bearer. Obtenga un token llamando POST /api/auth/login o POST /api/auth/register.\nEn Swagger UI haga clic en 'Authorize' y pegue: Bearer <su_token>.\nEl token JWT incluye el claim 'userId' (identificador numérico del usuario) y el rol (ClaimTypes.Role) que se usa para proteger endpoints."
    });

    // JWT Bearer
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingrese 'Bearer' seguido del token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new[] { "Bearer" } }
    });

    // Incluir comentarios XML si existe (necesita generar XML docs en csproj)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // No custom filters
});

// Configuración de EF Core
builder.Services.AddDbContext<TravelRequestsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de dependencias
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITravelRequestRepository, TravelRequestRepository>();
// Register email service implementation
builder.Services.AddScoped<IEmailService, ConsoleEmailService>();

// Register AuthService with DI (IEmailService will be injected)
builder.Services.AddScoped<AuthService>();
// Register TravelRequestService
builder.Services.AddScoped<ITravelRequestService, TravelRequestService>();

// Logging (use default ILogger or configure Serilog externally)

// Configuración de JWT (antes de app.Build)
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Travel Requests API v1");
        c.RoutePrefix = string.Empty; //Esto hace que Swagger sea la raíz
    });
}

app.UseHttpsRedirection();

// Exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
