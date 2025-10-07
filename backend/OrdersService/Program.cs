using OrdersService.Application.Abstractions;
using OrdersService.Application.Commands;
using OrdersService.Application.Queries;
using OrdersService.Infrastructure.Integrations;
using OrdersService.Infrastructure.Messaging;
using OrdersService.Infrastructure.Persistence;
using OrdersService.Infrastructure.Routing;
using OrdersService.Infrastructure.Time;
using OrdersService.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation();

builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddSingleton<IEventPublisher, InMemoryEventPublisher>();
builder.Services.AddSingleton<IEventStore>(sp => (IEventStore)sp.GetRequiredService<IEventPublisher>());
builder.Services.AddSingleton<IRestaurantIntegration, FakeRestaurantIntegration>();
builder.Services.AddSingleton<IRiderAssignmentService, InMemoryRiderAssignmentService>();
builder.Services.AddSingleton<IOrderCommandService, OrderCommandService>();
builder.Services.AddSingleton<IOrderQueryService, OrderQueryService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPresentationEndpoints();

app.Run();