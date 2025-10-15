using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportsService.Application.Abstractions;
using ReportsService.Application.MaterializedViews;

namespace ReportsService.Infrastructure.Bootstrapping;

public sealed class ReportViewBootstrapper : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public ReportViewBootstrapper(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IReportEventRepository>();
        var viewStore = scope.ServiceProvider.GetRequiredService<ReportMaterializedViewStore>();

        var events = await repository.LoadAllAsync(cancellationToken).ConfigureAwait(false);
        foreach (var @event in events)
        {
            viewStore.Apply(@event);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
