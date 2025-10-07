# NotificationsService

Servicio encargado de la capa de notificaciones en tiempo real. Actualmente actúa como stub para integrar mecanismos de mensajería y WebSockets.

## Endpoints principales

| Método | Ruta | Descripción |
| --- | --- | --- |
| `GET` | `/health` | Verificación básica del servicio. |

## Ejecución local

```bash
dotnet run --project NotificationsService.csproj
```

## Imagen Docker

```bash
docker build -t kinetra-notifications:latest .
docker run --rm -p 5006:8080 kinetra-notifications:latest
```
