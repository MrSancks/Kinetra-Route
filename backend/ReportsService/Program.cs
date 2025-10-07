using ReportsService.Application.MaterializedViews;
using ReportsService.Application.Services;
using ReportsService.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ReportMaterializedViewStore>();
builder.Services.AddSingleton<ReportIngestionService>();
builder.Services.AddSingleton<ReportQueryService>();
builder.Services.AddSingleton<ReportExportService>();

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
