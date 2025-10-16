using Microsoft.EntityFrameworkCore;
using TravelRequests.Infrastructure.Data;
using TravelRequests.Infrastructure.Repositories;
using TravelRequests.Application.Interfaces;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración EF Core
builder.Services.AddDbContext<TravelRequestsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de dependencias
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITravelRequestRepository, TravelRequestRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwagger();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
