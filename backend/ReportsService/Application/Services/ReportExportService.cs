using System.Text;
using ReportsService.Application.Utilities;
using ReportsService.Contracts.Responses;
using ReportsService.Domain.Enums;

namespace ReportsService.Application.Services;

public sealed class ReportExportService
{
    private readonly ReportQueryService _queryService;

    public ReportExportService(ReportQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<ReportExportResult> ExportAsync(string format, PeriodGranularity granularity, DateOnly? start, DateOnly? end, CancellationToken cancellationToken = default)
    {
        var normalizedFormat = (format ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(normalizedFormat))
        {
            throw new ArgumentException("El formato de exportación es obligatorio", nameof(format));
        }

        var orders = await _queryService.GetOrdersReportAsync(granularity, start, end, cancellationToken);
        var riders = await _queryService.GetRiderPerformanceAsync(null, cancellationToken);
        var revenue = await _queryService.GetRevenueAnalysisAsync(granularity, start, end, cancellationToken);

        return normalizedFormat switch
        {
            "pdf" => BuildPdfExport(orders, riders, revenue),
            "excel" or "csv" => BuildCsvExport(orders, riders, revenue),
            _ => throw new ArgumentException($"Formato '{format}' no soportado. Use 'pdf' o 'excel'.", nameof(format))
        };
    }

    private static ReportExportResult BuildPdfExport(OrdersReportResponse orders, RiderPerformanceResponse riders, RevenueAnalysisResponse revenue)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Reporte consolidado de Kinetra-Route");
        builder.AppendLine(new string('=', 40));
        builder.AppendLine();

        AppendOrdersSection(builder, orders);
        builder.AppendLine();
        AppendRidersSection(builder, riders);
        builder.AppendLine();
        AppendRevenueSection(builder, revenue);

        var payload = PdfBuilder.FromText(builder.ToString());
        var fileName = $"reporte_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
        return new ReportExportResult(payload, "application/pdf", fileName);
    }

    private static ReportExportResult BuildCsvExport(OrdersReportResponse orders, RiderPerformanceResponse riders, RevenueAnalysisResponse revenue)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Sección;Periodo;Inicio;Fin;Pedidos;Ingresos Brutos;Ingresos Plataforma;Adicional");

        foreach (var period in orders.Periods)
        {
            builder.AppendLine(string.Join(';',
                "Pedidos",
                period.Period,
                period.RangeStart.ToString("yyyy-MM-dd"),
                period.RangeEnd.ToString("yyyy-MM-dd"),
                period.Orders,
                period.GrossRevenue.ToString("F2"),
                period.PlatformRevenue.ToString("F2"),
                string.Empty));
        }

        foreach (var rider in riders.Riders)
        {
            builder.AppendLine(string.Join(';',
                "Repartidores",
                rider.RiderId,
                string.Empty,
                string.Empty,
                rider.CompletedOrders,
                rider.TotalRevenueGenerated.ToString("F2"),
                rider.OnTimeRate.ToString("P2"),
                rider.AverageDeliveryDurationMinutes?.ToString("F2") ?? string.Empty));
        }

        foreach (var breakdown in revenue.Breakdown)
        {
            builder.AppendLine(string.Join(';',
                "Ingresos",
                breakdown.Period,
                breakdown.RangeStart.ToString("yyyy-MM-dd"),
                breakdown.RangeEnd.ToString("yyyy-MM-dd"),
                breakdown.Orders,
                breakdown.GrossRevenue.ToString("F2"),
                breakdown.PlatformRevenue.ToString("F2"),
                string.Empty));
        }

        var payload = Encoding.UTF8.GetBytes(builder.ToString());
        var fileName = $"reporte_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        return new ReportExportResult(payload, "text/csv", fileName);
    }

    private static void AppendOrdersSection(StringBuilder builder, OrdersReportResponse orders)
    {
        builder.AppendLine("Pedidos");
        builder.AppendLine(new string('-', 20));
        builder.AppendLine($"Granularidad: {orders.Granularity}");

        foreach (var period in orders.Periods)
        {
            builder.AppendLine($" - {period.Period}: {period.Orders} pedidos, ingresos brutos {period.GrossRevenue:C2}, plataforma {period.PlatformRevenue:C2}");
        }
    }

    private static void AppendRidersSection(StringBuilder builder, RiderPerformanceResponse riders)
    {
        builder.AppendLine("Desempeño de repartidores");
        builder.AppendLine(new string('-', 20));

        foreach (var rider in riders.Riders)
        {
            var onTimePercentage = rider.OnTimeRate.ToString("P2");
            var averageDuration = rider.AverageDeliveryDurationMinutes is null ? "N/D" : $"{rider.AverageDeliveryDurationMinutes:F2} min";
            builder.AppendLine($" - {rider.RiderId}: {rider.CompletedOrders} entregas, puntualidad {onTimePercentage}, duración promedio {averageDuration}");
        }
    }

    private static void AppendRevenueSection(StringBuilder builder, RevenueAnalysisResponse revenue)
    {
        builder.AppendLine("Ingresos");
        builder.AppendLine(new string('-', 20));
        builder.AppendLine($"Total bruto: {revenue.TotalGrossRevenue:C2}");
        builder.AppendLine($"Total plataforma: {revenue.TotalPlatformRevenue:C2}");
        builder.AppendLine($"Pedidos completados: {revenue.TotalCompletedOrders}");

        foreach (var period in revenue.Breakdown)
        {
            builder.AppendLine($" - {period.Period}: {period.Orders} pedidos, bruto {period.GrossRevenue:C2}, plataforma {period.PlatformRevenue:C2}");
        }
    }
}

public sealed record ReportExportResult(byte[] Content, string ContentType, string FileName);
