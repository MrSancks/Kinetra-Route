using System.Threading;
using System.Threading.Tasks;
using RoutesService.Application.Contracts;

namespace RoutesService.Application.Services;

public interface IEtaService
{
    Task<EtaResponse?> GetEtaAsync(string deliveryId, string riderId, CancellationToken cancellationToken);
}
