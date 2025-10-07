# Backend Microservices

Este directorio contiene los proyectos base de los microservicios de Kinetra-Route. Cada servicio es un proyecto independiente de ASP.NET Core 8 con su propia capa de presentación y documentación.

## Servicios incluidos

| Servicio | Descripción | Documentación |
| --- | --- | --- |
| `AuthService` | Autenticación, emisión y refresco de tokens JWT. | [README](./AuthService/README.md) |
| `RidersService` | Administración de repartidores y disponibilidad. | [README](./RidersService/README.md) |
| `OrdersService` | Gestión del ciclo de vida de pedidos y eventos de dominio. | [README](./OrdersService/README.md) |
| `RoutesService` | Base para optimización y seguimiento de rutas. | [README](./RoutesService/README.md) |
| `PaymentsService` | Cálculo de tarifas y liquidaciones. | [README](./PaymentsService/README.md) |
| `NotificationsService` | Stub para las notificaciones en tiempo real. | [README](./NotificationsService/README.md) |
| `ReportsService` | Analítica y reportes consolidados. | [README](./ReportsService/README.md) |

Cada proyecto puede ejecutarse de forma independiente con `dotnet run` dentro de la carpeta correspondiente (ver documentación individual).

## Ejecución con Docker

Todos los servicios incluyen un `Dockerfile` multi-stage. Para construirlos de forma manual:

```bash
docker build -t kinetra-<servicio> ./<NombreServicio>
docker run --rm -p <puerto_host>:8080 kinetra-<servicio>
```

## Orquestación con Docker Compose

Se incluye un archivo [`docker-compose.yml`](./docker-compose.yml) que levanta los siete microservicios y un gateway Nginx que los enruta en un mismo host.

```bash
docker compose up --build
```

Puertos expuestos por defecto:

| Servicio | Puerto |
| --- | --- |
| Auth | `5001` |
| Riders | `5002` |
| Orders | `5003` |
| Routes | `5004` |
| Payments | `5005` |
| Notifications | `5006` |
| Reports | `5007` |
| Gateway | `8080` |

El gateway enruta prefijos como `/auth`, `/orders`, `/riders`, etc. hacia los contenedores internos, facilitando las pruebas integradas.
