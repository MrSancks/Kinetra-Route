# RidersService

Administra la información de repartidores, su disponibilidad y el historial de entregas.

## Endpoints principales

| Método | Ruta | Descripción |
| --- | --- | --- |
| `GET` | `/health` | Comprobación del servicio. |
| `GET` | `/api/riders/` | Lista los repartidores registrados. |
| `GET` | `/api/riders/available` | Muestra únicamente a los repartidores disponibles. |
| `GET` | `/api/riders/{id}` | Recupera un repartidor específico. |
| `POST` | `/api/riders/` | Crea un nuevo repartidor. |
| `PUT` | `/api/riders/{id}` | Actualiza los datos generales de un repartidor. |
| `DELETE` | `/api/riders/{id}` | Elimina a un repartidor. |
| `PATCH` | `/api/riders/{id}/availability` | Cambia el estado de disponibilidad. |
| `GET` | `/api/riders/{id}/deliveries` | Consulta el historial de entregas. |
| `POST` | `/api/riders/{id}/deliveries` | Registra una nueva entrega. |
| `POST` | `/api/riders/{id}/validate-documents` | Lanza la validación de documentos. |

## Ejecución local

```bash
dotnet run --project RidersService.csproj
```

## Imagen Docker

```bash
docker build -t kinetra-riders:latest .
docker run --rm -p 5002:8080 kinetra-riders:latest
```
