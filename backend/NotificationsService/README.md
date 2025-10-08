# NotificationsService

Servicio encargado de orquestar y entregar notificaciones en tiempo real para repartidores y restaurantes.
Implementa un enfoque *event-driven* apoyado en el patrón **Outbox** para asegurar la consistencia entre los
comandos HTTP entrantes y los eventos de entrega que se publican hacia los canales externos (push, correo y SMS).

## Arquitectura

- **API minimal** para recibir eventos de negocio desde otros servicios (nuevos pedidos, cambios de estado, alertas).
- **EF Core InMemory** actúa como almacén transaccional tanto para las notificaciones como para los mensajes del outbox.
- **Hosted service `OutboxProcessor`** que publica periódicamente los eventos pendientes y marca las notificaciones como entregadas.
- **Patrón Outbox**: cada notificación genera un registro en `OutboxMessages` que es procesado de forma asíncrona, evitando
  pérdidas de mensajes ante fallos en los canales externos.

## Endpoints principales

| Método | Ruta | Descripción |
| --- | --- | --- |
| `GET` | `/health` | Verificación básica del servicio. |
| `POST` | `/api/notifications/orders/new` | Genera notificaciones de nuevo pedido para restaurante y repartidor. |
| `POST` | `/api/notifications/orders/status` | Informa cambios de estado de entrega a los destinatarios configurados. |
| `POST` | `/api/notifications/orders/alerts` | Emite alertas críticas (cancelación, cambio de ruta, mensajes personalizados). |
| `GET` | `/api/notifications/pending` | Consulta las notificaciones pendientes de enviar. |
| `GET` | `/api/notifications/outbox/pending` | Consulta los mensajes del outbox aún no procesados. |

## Ejecución local

```bash
dotnet run --project NotificationsService.csproj
```

## Imagen Docker

```bash
docker build -t kinetra-notifications:latest .
docker run --rm -p 5006:8080 kinetra-notifications:latest
```
