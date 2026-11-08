# Esquema de base de datos

## Vista de consulta

```sql
CREATE VIEW VistaTickets AS
SELECT
    T.Id AS TicketId,
    T.Titulo,
    T.Descripcion,
    T.Categoria,
    E.Nombre AS Estado,
    S.Nombre AS Solicitante,
    S.Email AS EmailSolicitante,
    A.Nombre AS Asignado,
    T.FechaCreacion,
    T.FechaActualizacion,
    T.FechaResolucion,
    (SELECT COUNT(*) FROM Comentario C WHERE C.IdTicket = T.Id) AS NumComentarios
FROM Ticket T
INNER JOIN EstadoTicket E ON T.IdEstado = E.Id
INNER JOIN Usuario S ON T.IdUsuarioSolicitante = S.Id
LEFT JOIN Usuario A ON T.IdUsuarioAsignado = A.Id;
```

`Asignado` queda en `NULL` cuando el ticket no tiene usuario asignado. La vista se puede usar para consultas de reportes, pero la aplicación genera sus proyecciones directamente con Entity Framework.

## Modelo DBML

```dbml
Table EstadoTicket [headercolor: #FFD800] {
	Id integer [pk, increment, not null]
	Nombre varchar(50) [not null, unique]
}

Table Comentario [headercolor: #FFD800] {
	Id integer [pk, increment, not null]
	IdTicket integer [not null]
	IdUsuario integer [not null]
	Texto varchar(255) [not null]
	Fecha datetime [not null]
}

Table Ticket [headercolor: #FFD800] {
	Id integer [pk, increment, not null]
	Titulo varchar(150) [not null]
	Descripcion varchar(255) [not null]
	IdUsuarioSolicitante integer [not null]
	IdUsuarioAsignado integer
	IdEstado integer [not null]
	FechaCreacion datetime [not null]
	FechaActualizacion datetime [not null]
	FechaResolucion datetime
	Categoria varchar(100)
}

Table Usuario [headercolor: #FFD800] {
	Id integer [pk, increment, not null]
	Nombre varchar(100) [not null]
	Email varchar(150) [not null, unique]
	Hash varbinary(255) [not null]
	Rol varchar(50) [not null]
	FechaRegistro datetime [not null]
	Estado varchar(30) [not null]
}

Ref EstadoClasificaTicket {
	Ticket.IdEstado > EstadoTicket.Id [delete: no action, update: no action]
}

Ref UsuarioAsignaTicket {
	Usuario.Id < Ticket.IdUsuarioAsignado [delete: no action, update: no action]
}

Ref UsuarioSolicitaTicket {
	Usuario.Id < Ticket.IdUsuarioSolicitante [delete: no action, update: no action]
}

Ref UsuarioEscribeComentario {
	Usuario.Id < Comentario.IdUsuario [delete: no action, update: no action]
}

Ref TicketRecibeComentario {
	Ticket.Id < Comentario.IdTicket [delete: cascade, update: no action]
}
```

## Índices

Entity Framework genera los siguientes índices:

- `Usuario.Email`: único, nombre `IX_Usuario_Email`.
- `EstadoTicket.Nombre`: único, nombre `IX_EstadoTicket_Nombre`.
- `Ticket.IdUsuarioSolicitante`: `IX_Ticket_IdUsuarioSolicitante`.
- `Ticket.IdUsuarioAsignado`: `IX_Ticket_IdUsuarioAsignado`.
- `Ticket.IdEstado`: `IX_Ticket_IdEstado`.
- `Comentario.IdTicket`: `IX_Comentario_IdTicket`.
- `Comentario.IdUsuario`: `IX_Comentario_IdUsuario`.

## Reglas y decisiones

- `Categoria` es opcional; la aplicación la valida sólo por longitud máxima de 100 caracteres.
- `FechaResolucion` es opcional. Se asigna al pasar a `Resuelto` o `Cerrado`, se conserva al cerrar un ticket ya resuelto y se limpia al reabrir.
- `IdUsuarioAsignado` es opcional; un ticket sin asignar se muestra como “Sin asignar”.
- Al eliminar un ticket se eliminan sus comentarios en cascada.
- Las relaciones con `Usuario` no eliminan usuarios ni tickets en cascada.
- Los estados iniciales son `Abierto`, `En Proceso`, `Resuelto` y `Cerrado`.
- El diagrama visual se encuentra en `Diagrama.png`. Este script es la fuente canónica del esquema.
