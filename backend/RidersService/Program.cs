using System;
using Microsoft.EntityFrameworkCore;
using RidersService.Application.Services;
using RidersService.Infrastructure.Data;
using RidersService.Infrastructure.Repositories;
using RidersService.Infrastructure.UnitOfWork;
using RidersService.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' is not configured for RidersService.");

builder.Services.AddDbContext<RidersDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IRiderRepository, RiderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDocumentValidationService, DocumentValidationService>();
builder.Services.AddScoped<IRiderService, RiderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthEndpoints();
app.MapRiderEndpoints();

app.Run();
