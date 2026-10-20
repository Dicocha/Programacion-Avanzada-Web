# Programación Avanzada Web

Repositorio del curso **Programación Avanzada en Web** (sección 03101) de la
UNED. Concentra las entregas del curso —tareas y proyectos— y funciona como
demostración práctica de trabajo con Git: historial limpio, un commit por
funcionalidad y una rama por entrega.

## Objetivo

- Aplicar desarrollo web en capas (**Domain**, **Application**,
  **Infrastructure**, **Presentation**) sobre .NET.
- Demostrar manejo profesional de Git: **ramas por entrega**, **commits
  atómicos** e historial fácil de revisar.
- Facilitar al docente la revisión de cada trabajo entrando directo a su rama.

## Estructura de ramas

| Rama         | Contenido                                                  | Estado              |
|--------------|------------------------------------------------------------|---------------------|
| `master`     | Solo este README (punto de partida y referencia del repo)  | Activa              |
| `tarea-1`    | Código fuente de la Tarea 1 — Web Forms, solución en capas | Entregada           |
| `tarea-2`    | Código fuente de la Tarea 2 — ASP.NET Core MVC (To-Do List)| Entregada           |
| `proyecto-1` | —                                                          | Esperando enunciado |
| `proyecto-2` | —                                                          | Esperando enunciado |

Notas sobre el versionamiento:

- Cada rama versiona **solo el código** que estaba en su carpeta `src/`
  (solución, proyectos y source files). **No** se versionan `bin/`, `obj/`,
  `.vs/` ni `packages/`: son artefactos generados o restaurables.
- Los documentos de entrega (documento técnico, diagramas, videos) y los
  scripts `commits.sh` permanecen fuera de Git, junto al código en la copia
  local de trabajo.

## Clonar y compilar

### Requisitos

- Visual Studio 2022 con el workload **ASP.NET y desarrollo web**
- .NET 8 SDK (Tarea 2)
- NuGet CLI (solo Tarea 1, si VS no restaura los paquetes automáticamente)

### Tarea 2 — ASP.NET Core MVC (.NET 8)

```bash
git clone https://github.com/Dicocha/Programacion-Avanzada-Web.git
cd Programacion-Avanzada-Web
git checkout tarea-2
dotnet restore
dotnet run --project Presentation/MVC.csproj
```

### Tarea 1 — Web Forms (.NET Framework)

```bash
git checkout tarea-1
nuget restore "Tarea 1.slnx"
```

Luego abrir `Tarea 1.slnx` en Visual Studio 2022 y compilar con **Ctrl+Shift+B**.
