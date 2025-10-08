using Microsoft.EntityFrameworkCore;
using NotificationsService.Application.Services;
using NotificationsService.BackgroundTasks;
using NotificationsService.Infrastructure.Data;
using NotificationsService.Infrastructure.Services;
using NotificationsService.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<NotificationsDbContext>(options =>
    options.UseInMemoryDatabase("NotificationsServiceDb"));

builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddHostedService<OutboxProcessor>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthEndpoints();
app.MapNotificationEndpoints();

app.Run();
