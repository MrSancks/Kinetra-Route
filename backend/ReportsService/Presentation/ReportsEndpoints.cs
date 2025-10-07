using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReportsService.Application.Services;
using ReportsService.Contracts.Requests;
using ReportsService.Domain.Enums;

namespace ReportsService.Presentation;

public static class ReportsEndpoints
{
    public static void MapReportIngestionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/events")
            .WithTags("Report Events");

        group.MapPost("/order-completed", async ([FromBody] RecordOrderCompletionRequest request, ReportIngestionService ingestionService, CancellationToken cancellationToken) =>
        {
            await ingestionService.RecordOrderCompletionAsync(request, cancellationToken);
            return Results.Accepted($"/reports/orders/{request.OrderId}", new { request.OrderId });
        })
        .WithName("RecordOrderCompletion")
        .Produces(StatusCodes.Status202Accepted);
    }

    public static void MapReportsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/reports")
            .WithTags("Reports");

        group.MapGet("/orders", async ([FromQuery] PeriodGranularity? granularity, [FromQuery] DateOnly? start, [FromQuery] DateOnly? end, ReportQueryService queryService, CancellationToken cancellationToken) =>
        {
            if (!ValidateRange(start, end, out var problem))
            {
                return Results.Problem(problem);
            }

            var selectedGranularity = granularity ?? PeriodGranularity.Daily;
            var report = await queryService.GetOrdersReportAsync(selectedGranularity, start, end, cancellationToken);
            return Results.Ok(report);
        })
        .WithName("GetOrdersReport")
        .Produces(StatusCodes.Status200OK);

        group.MapGet("/riders/performance", async ([FromQuery] string? riderId, ReportQueryService queryService, CancellationToken cancellationToken) =>
        {
            var report = await queryService.GetRiderPerformanceAsync(riderId, cancellationToken);
            return Results.Ok(report);
        })
        .WithName("GetRiderPerformance")
        .Produces(StatusCodes.Status200OK);

        group.MapGet("/revenue", async ([FromQuery] PeriodGranularity? granularity, [FromQuery] DateOnly? start, [FromQuery] DateOnly? end, ReportQueryService queryService, CancellationToken cancellationToken) =>
        {
            if (!ValidateRange(start, end, out var problem))
            {
                return Results.Problem(problem);
            }

            var selectedGranularity = granularity ?? PeriodGranularity.Monthly;
            var report = await queryService.GetRevenueAnalysisAsync(selectedGranularity, start, end, cancellationToken);
            return Results.Ok(report);
        })
        .WithName("GetRevenueAnalysis")
        .Produces(StatusCodes.Status200OK);

        group.MapGet("/export", async ([FromQuery] string format, [FromQuery] PeriodGranularity? granularity, [FromQuery] DateOnly? start, [FromQuery] DateOnly? end, ReportExportService exportService, CancellationToken cancellationToken) =>
        {
            if (!ValidateRange(start, end, out var problem))
            {
                return Results.Problem(problem);
            }

            var selectedGranularity = granularity ?? PeriodGranularity.Monthly;
            try
            {
                var export = await exportService.ExportAsync(format, selectedGranularity, start, end, cancellationToken);
                return Results.File(export.Content, export.ContentType, export.FileName);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Formato inválido",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
        })
        .WithName("ExportReports")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }

    private static bool ValidateRange(DateOnly? start, DateOnly? end, out ProblemDetails? problem)
    {
        problem = null;

        if (start.HasValue && end.HasValue && end < start)
        {
            problem = new ProblemDetails
            {
                Title = "Rango inválido",
                Detail = "La fecha final debe ser mayor o igual a la inicial.",
                Status = StatusCodes.Status400BadRequest
            };
            return false;
        }

        return true;
    }
}
