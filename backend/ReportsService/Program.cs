using System;
using Microsoft.EntityFrameworkCore;
using ReportsService.Application.Abstractions;
using ReportsService.Application.MaterializedViews;
using ReportsService.Application.Services;
using ReportsService.Infrastructure.Bootstrapping;
using ReportsService.Infrastructure.Data;
using ReportsService.Infrastructure.Repositories;
using ReportsService.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ReportMaterializedViewStore>();

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' is not configured for ReportsService.");

builder.Services.AddDbContext<ReportsDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IReportEventRepository, ReportEventRepository>();
builder.Services.AddScoped<ReportIngestionService>();
builder.Services.AddSingleton<ReportQueryService>();
builder.Services.AddSingleton<ReportExportService>();
builder.Services.AddHostedService<ReportViewBootstrapper>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthEndpoints();
app.MapReportIngestionEndpoints();
app.MapReportsEndpoints();

app.Run();
