# ReportsService

Servicio responsable de la analítica y generación de reportes agregados.

## Endpoints principales

| Método | Ruta | Descripción |
| --- | --- | --- |
| `GET` | `/health` | Verificación del servicio. |
| `POST` | `/events/order-completed` | Registra un evento de pedido completado. |
| `GET` | `/reports/orders` | Consulta reportes de pedidos agregados. |
| `GET` | `/reports/riders/performance` | Mide el desempeño de repartidores. |
| `GET` | `/reports/revenue` | Reporte de ingresos por periodo. |
| `GET` | `/reports/export` | Exporta los reportes en diferentes formatos. |

## Ejecución local

```bash
dotnet run --project ReportsService.csproj
```

## Imagen Docker

```bash
docker build -t kinetra-reports:latest .
docker run --rm -p 5007:8080 kinetra-reports:latest
```
