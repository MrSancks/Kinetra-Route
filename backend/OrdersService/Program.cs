using System;
using Microsoft.EntityFrameworkCore;
using OrdersService.Application.Abstractions;
using OrdersService.Application.Commands;
using OrdersService.Application.Queries;
using OrdersService.Infrastructure.Data;
using OrdersService.Infrastructure.Integrations;
using OrdersService.Infrastructure.Messaging;
using OrdersService.Infrastructure.Persistence;
using OrdersService.Infrastructure.Routing;
using OrdersService.Infrastructure.Time;
using OrdersService.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation();

builder.Services.AddSingleton<IClock, SystemClock>();

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' is not configured for OrdersService.");

builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IEventPublisher, InMemoryEventPublisher>();
builder.Services.AddSingleton<IEventStore>(sp => (IEventStore)sp.GetRequiredService<IEventPublisher>());
builder.Services.AddSingleton<IRestaurantIntegration, FakeRestaurantIntegration>();
builder.Services.AddSingleton<IRiderAssignmentService, InMemoryRiderAssignmentService>();
builder.Services.AddScoped<IOrderCommandService, OrderCommandService>();
builder.Services.AddScoped<IOrderQueryService, OrderQueryService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPresentationEndpoints();

app.Run();
