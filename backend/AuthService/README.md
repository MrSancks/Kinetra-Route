# AuthService

Servicio responsable de la autenticación y gestión de usuarios para Kinetra-Route. Expone endpoints de registro, inicio de sesión y refresco de tokens JWT.

## Endpoints principales

| Método | Ruta | Descripción |
| --- | --- | --- |
| `GET` | `/health` | Verificación de estado del servicio. |
| `POST` | `/api/auth/register/rider` | Registra un nuevo repartidor. |
| `POST` | `/api/auth/register/restaurant` | Registra un nuevo restaurante. |
| `POST` | `/api/auth/register/admin` | Registra un administrador (requiere rol `Admin`). |
| `POST` | `/api/auth/login` | Inicia sesión y entrega tokens JWT. |
| `POST` | `/api/auth/refresh` | Refresca un token JWT expirado. |
| `GET` | `/api/auth/logs` | Consulta logs de acceso (requiere rol `Admin`). |

## Ejecución local

```bash
dotnet run --project AuthService.csproj
```

Por defecto el servicio se expone en `http://localhost:5085` cuando se ejecuta con el SDK.

## Imagen Docker

El proyecto incluye un `Dockerfile` multi-stage. Para construir y ejecutar el contenedor de forma individual:

```bash
docker build -t kinetra-auth:latest .
docker run --rm -p 5001:8080 kinetra-auth:latest
```

El contenedor expone el puerto `8080` internamente y escucha en `http://0.0.0.0:8080`.
