using PaymentsService.Application.Abstractions;
using PaymentsService.Application.Services;
using PaymentsService.Domain.Services;
using PaymentsService.Domain.ValueObjects;
using PaymentsService.Infrastructure.Configuration;
using PaymentsService.Infrastructure.Time;
using PaymentsService.Presentation;
using PaymentsService.Presentation.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<PaymentPolicyOptions>(builder.Configuration.GetSection(PaymentPolicyOptions.SectionName));
builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<PaymentPolicyOptions>>().Value;
    return PaymentPolicy.Create(
        options.BaseFee,
        options.PerKilometerRate,
        options.PlatformCommissionRate,
        options.LongDistanceThresholdKm,
        options.LongDistanceBonus);
});

builder.Services.AddSingleton<PaymentDomainService>();
builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddScoped<PaymentApplicationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthEndpoints();
app.MapPaymentEndpoints();

app.Run();
