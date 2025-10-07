# RoutesService

Servicio destinado a la optimización y seguimiento de rutas. Actualmente sirve como base para ampliar funcionalidades de geolocalización.

## Endpoints principales

| Método | Ruta | Descripción |
| --- | --- | --- |
| `GET` | `/health` | Verifica el estado del servicio. |

## Ejecución local

```bash
dotnet run --project RoutesService.csproj
```

## Imagen Docker

```bash
docker build -t kinetra-routes:latest .
docker run --rm -p 5004:8080 kinetra-routes:latest
```
