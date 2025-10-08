using System;

namespace RoutesService.Application.Contracts;

public sealed record EtaResponse(
    string DeliveryId,
    string RiderId,
    DateTimeOffset EstimatedArrival,
    TimeSpan RemainingDuration,
    double ProgressPercentage);
