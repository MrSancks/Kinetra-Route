# RoutesService

Servicio destinado a la optimización y seguimiento de rutas de entrega siguiendo una arquitectura hexagonal (Ports & Adapters). Expone capacidades para calcular rutas óptimas, hacer tracking GPS en tiempo real, estimar tiempos de llegada y ofrecer datos para la app móvil de los repartidores.

## Endpoints principales

| Método | Ruta | Descripción |
| --- | --- | --- |
| `GET` | `/health` | Verifica el estado del servicio. |
| `POST` | `/routes/optimize` | Calcula una ruta óptima usando datos de tráfico simulados. |
| `GET` | `/tracking/{riderId}` | Devuelve la última ubicación reportada por el repartidor. |
| `POST` | `/tracking/{riderId}` | Registra/actualiza la ubicación proveniente de la app móvil. |
| `GET` | `/deliveries/{deliveryId}/eta?riderId={riderId}` | Calcula el ETA combinando ruta y posición actual del repartidor. |

## Arquitectura

El servicio está organizado en capas siguiendo el patrón de arquitectura hexagonal:

- **Domain**: Entidades y objetos de valor que describen planes de ruta, paradas y ubicaciones.
- **Application**: Casos de uso y contratos expuestos a la capa de presentación. Define los puertos hacia infraestructura.
- **Infrastructure**: Adaptadores que proveen implementaciones concretas como el cliente stub de rutas y repositorios en memoria.
- **Presentation**: Endpoints mínimos expuestos vía ASP.NET Core Minimal APIs.

Esta estructura permite sustituir fácilmente la integración con APIs externas de mapas (OpenStreetMap/Google Maps) o almacenes de datos reales sin afectar la lógica del dominio.

## Ejecución local

```bash
dotnet run --project RoutesService.csproj
```

## Imagen Docker

```bash
docker build -t kinetra-routes:latest .
docker run --rm -p 5004:8080 kinetra-routes:latest
```
