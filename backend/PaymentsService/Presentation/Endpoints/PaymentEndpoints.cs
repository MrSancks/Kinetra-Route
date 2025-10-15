using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using PaymentsService.Application.Services;
using PaymentsService.Contracts.Requests;

namespace PaymentsService.Presentation.Endpoints;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/payments").WithTags("Payments");

        group.MapPost("/tariff", async (CalculateTariffRequest request, PaymentApplicationService service, CancellationToken cancellationToken) =>
        {
            var response = await service.CalculateOrderTariffAsync(request, cancellationToken);
            return Results.Ok(response);
        })
        .WithName("CalculateOrderTariff")
        .Produces(StatusCodes.Status200OK);

        group.MapPost("/settlements/weekly", async (WeeklySettlementRequest request, PaymentApplicationService service, CancellationToken cancellationToken) =>
        {
            var response = await service.GenerateWeeklySettlementAsync(request, cancellationToken);
            return Results.Ok(response);
        })
        .WithName("GenerateWeeklySettlement")
        .Produces(StatusCodes.Status200OK);

        group.MapPost("/platform/commission", async (PlatformCommissionRequest request, PaymentApplicationService service, CancellationToken cancellationToken) =>
        {
            var response = await service.CalculatePlatformCommissionAsync(request, cancellationToken);
            return Results.Ok(response);
        })
        .WithName("CalculatePlatformCommission")
        .Produces(StatusCodes.Status200OK);

        group.MapPost("/reports/realtime", async (RealTimeReportRequest request, PaymentApplicationService service, CancellationToken cancellationToken) =>
        {
            var response = await service.GenerateRealTimeReportAsync(request, cancellationToken);
            return Results.Ok(response);
        })
        .WithName("GenerateRealTimeReport")
        .Produces(StatusCodes.Status200OK);
    }
}
