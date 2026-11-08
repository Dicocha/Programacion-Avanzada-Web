# Help Desk - Gestión de Solicitudes de Soporte

Prototipo funcional de un sistema Help Desk desarrollado con ASP.NET MVC 5 y .NET Framework 4.8. Permite que los empleados registren tickets de soporte y que los administradores asignen, atiendan, cambien de estado y consulten los tickets del área.

## Repositorio y estructura

Repositorio:

```text
https://github.com/Dicocha/Programacion-Avanzada-Web.git
```

Rama de trabajo:

```text
proyecto-1
```

La raíz del repositorio es la carpeta `src` del proyecto local, no la carpeta `proyecto-1`:

```text
...\proyecto-1\src
```

No se debe inicializar Git en `proyecto-1`, porque el proyecto quedaría como `src/App_Start/...`. El repositorio padre mantiene `proyecto-1/` en `.git/info/exclude` para evitar un repositorio anidado o un submódulo accidental.

La rama remota `proyecto-1` existía inicialmente como un commit vacío (`a9e28dc`). El repositorio independiente se creó directamente en `src` y se conectó a esa rama.

## Requisitos

- Visual Studio 2026 con el workload **ASP.NET and web development**.
- .NET Framework 4.8.
- SQL Server LocalDB.
- NuGet.
- Navegador moderno con JavaScript habilitado.

No se debe usar `dotnet build`: es un proyecto ASP.NET MVC 5 sobre .NET Framework, no ASP.NET Core.

## Puesta en marcha

Desde la carpeta `src`:

```powershell
nuget restore Help-Desk.sln
sqllocaldb create HelpDeskLocalDb
sqllocaldb start HelpDeskLocalDb
```

Si la instancia ya existe, no es necesario ejecutar `create`; alcanza con iniciarla.

Luego:

1. Abrir `Help-Desk.sln` en Visual Studio.
2. Verificar que el proyecto de inicio sea `HelpDesk.Web`.
3. Ejecutar con F5.
4. IIS Express abre la aplicación en:

```text
http://localhost:5105/
```

La conexión se llama `HelpDeskContext` y está definida en `Web.config`. Usa la instancia nombrada `HelpDeskLocalDb` y la base `HelpDeskDb`.

La aplicación aplica las migraciones automáticamente al iniciar y ejecuta el seed de datos.

## Usuarios de prueba

| Usuario | Contraseña | Rol |
| --- | --- | --- |
| `admin` | `admin` | Administrador |
| `usuario` | `usuario` | Usuario |

El login acepta nombre de usuario o correo electrónico.

## Funcionalidades

### Rol Usuario

- Crear tickets con título, descripción y categoría opcional.
- Consultar únicamente sus propios tickets.
- Ver el detalle y el estado actual.
- Agregar comentarios a sus tickets.

### Rol Administrador

- Ver todos los tickets del área.
- Asignar tickets a usuarios activos o a sí mismo.
- Cambiar el estado entre `Abierto`, `En Proceso`, `Resuelto` y `Cerrado`.
- Consultar el reporte general de tickets por estado.
- Ver el usuario asignado y el tiempo de resolución de cada ticket.
- Buscar y filtrar tickets.

## Operaciones AJAX

La aplicación utiliza AJAX real para:

- cargar y agregar comentarios sin recargar la página;
- actualizar el estado de un ticket sin recargar la página;
- asignar tickets sin recargar la página;
- filtrar el listado de tickets de forma instantánea;
- actualizar automáticamente el listado de tickets cada 5 segundos.

La actualización automática del listado permite que, si un ticket se crea en otra pestaña o por otro usuario, aparezca sin necesidad de recargar manualmente. El polling se pausa cuando la pestaña está oculta y se actualiza inmediatamente al volver a ella. No se utiliza SignalR para mantener el prototipo simple y sin dependencias adicionales.

## Base de datos

El esquema se documenta en:

- `Database/scriptDb.md`: script y modelo de referencia.
- `Database/Diagrama.png`: diagrama de la base de datos.

`scriptDb.md` es la fuente canónica del esquema. El diagrama puede ser una referencia visual y no necesariamente incluye todos los cambios más recientes.

Tablas principales:

- `Usuario`
- `Ticket`
- `Comentario`
- `EstadoTicket`

Estados iniciales: `Abierto`, `En Proceso`, `Resuelto` y `Cerrado`.

La categoría del ticket es opcional. La columna `FechaResolucion` se conserva al pasar de `Resuelto` a `Cerrado` y se limpia si el ticket se reabre.

## Migraciones

El proyecto usa EF6 Code First. `Global.asax.cs` configura `MigrateDatabaseToLatestVersion`, por lo que las migraciones se aplican al iniciar la aplicación.

Para cambios de modelo, en la Consola del Administrador de Paquetes con `HelpDesk.Web` seleccionado:

```powershell
Add-Migration NombreDeLaMigracion
Update-Database
```

El seed se encuentra en `Data/SeedData.cs` y se ejecuta desde `Migrations/Configuration.cs`.

## Seguridad

- La autenticación usa cookies OWIN (`HelpDeskCookie`), no ASP.NET Identity ni FormsAuthentication.
- Los roles se almacenan en `ClaimTypes.Role`.
- Los controladores usan `[Authorize]`.
- Las acciones de administración usan `[Authorize(Roles = "Administrador")]`.
- Los usuarios sólo pueden ver y comentar sus propios tickets.
- Las operaciones POST usan tokens antiforgery.
- Los reportes son exclusivos del rol Administrador.
