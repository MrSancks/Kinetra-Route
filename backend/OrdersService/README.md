# OrdersService

Gestiona el ciclo de vida de los pedidos dentro de Kinetra-Route, incluyendo creación, reasignación y cancelación, además de exponer un stream de eventos en memoria.

## Endpoints principales

| Método | Ruta | Descripción |
| --- | --- | --- |
| `GET` | `/health` | Estado del servicio. |
| `GET` | `/orders` | Obtiene todos los pedidos registrados. |
| `GET` | `/orders/{id}` | Recupera un pedido específico. |
| `POST` | `/orders` | Crea un nuevo pedido. |
| `PATCH` | `/orders/{id}/status` | Actualiza el estado de un pedido. |
| `POST` | `/orders/{id}/reassign` | Reasigna un pedido a otro repartidor. |
| `DELETE` | `/orders/{id}` | Cancela un pedido. |
| `GET` | `/events` | Devuelve los eventos de dominio generados (en memoria). |

## Ejecución local

```bash
dotnet run --project OrdersService.csproj
```

## Imagen Docker

```bash
docker build -t kinetra-orders:latest .
docker run --rm -p 5003:8080 kinetra-orders:latest
```
