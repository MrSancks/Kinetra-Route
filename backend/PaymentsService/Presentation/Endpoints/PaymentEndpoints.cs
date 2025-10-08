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

        group.MapPost("/tariff", (CalculateTariffRequest request, PaymentApplicationService service) =>
        {
            var response = service.CalculateOrderTariff(request);
            return Results.Ok(response);
        })
        .WithName("CalculateOrderTariff")
        .Produces(StatusCodes.Status200OK);

        group.MapPost("/settlements/weekly", (WeeklySettlementRequest request, PaymentApplicationService service) =>
        {
            var response = service.GenerateWeeklySettlement(request);
            return Results.Ok(response);
        })
        .WithName("GenerateWeeklySettlement")
        .Produces(StatusCodes.Status200OK);

        group.MapPost("/platform/commission", (PlatformCommissionRequest request, PaymentApplicationService service) =>
        {
            var response = service.CalculatePlatformCommission(request);
            return Results.Ok(response);
        })
        .WithName("CalculatePlatformCommission")
        .Produces(StatusCodes.Status200OK);

        group.MapPost("/reports/realtime", (RealTimeReportRequest request, PaymentApplicationService service) =>
        {
            var response = service.GenerateRealTimeReport(request);
            return Results.Ok(response);
        })
        .WithName("GenerateRealTimeReport")
        .Produces(StatusCodes.Status200OK);
    }
}
