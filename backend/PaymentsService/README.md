# PaymentsService

Servicio destinado al cálculo de tarifas, comisiones y liquidaciones bajo una arquitectura basada en dominios (DDD). Expone
funcionalidades para la gestión de pagos de repartidores, cálculo de costos de servicio y reportes en tiempo real.

## Endpoints principales

| Método | Ruta | Descripción |
| --- | --- | --- |
| `GET` | `/health` | Comprobación del estado del servicio. |
| `POST` | `/payments/tariff` | Calcula la tarifa total de un pedido a partir de distancia y política de pagos. |
| `POST` | `/payments/settlements/weekly` | Genera la liquidación semanal de un repartidor. |
| `POST` | `/payments/platform/commission` | Resume las comisiones que corresponden a la plataforma. |
| `POST` | `/payments/reports/realtime` | Obtiene un reporte agregado de ganancias en tiempo real. |

## Ejemplo de solicitud

```json
POST /payments/tariff
{
  "order": {
    "orderId": "ORD-1024",
    "riderId": "R-77",
    "distanceInKilometers": 8.2,
    "baseFee": 30.0,
    "completedAt": "2024-04-08T18:22:00Z"
  }
}
```

## Ejecución local

```bash
dotnet run --project PaymentsService.csproj
```

## Imagen Docker

```bash
docker build -t kinetra-payments:latest .
docker run --rm -p 5005:8080 kinetra-payments:latest
```
