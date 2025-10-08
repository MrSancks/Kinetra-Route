using RoutesService.Application;
using RoutesService.Infrastructure;
using RoutesService.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

builder.Services.AddRouteInfrastructure();
builder.Services.AddRouteApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapHealthEndpoints();
app.MapRouteEndpoints();
app.MapTrackingEndpoints();
app.MapEtaEndpoints();

app.Run();
