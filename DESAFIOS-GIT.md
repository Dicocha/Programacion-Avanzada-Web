# Desafío Git: repositorio anidado y raíz correcta del proyecto

## Contexto

El repositorio del curso es:

```text
https://github.com/Dicocha/Programacion-Avanzada-Web.git
```

La organización usada en las ramas existentes es que cada entrega tiene el proyecto en la raíz de la rama. Por ejemplo:

- `tarea-1`: `Application/`, `Domain/`, `Infrastructure/`, `Presentation/`, `Tarea 1.slnx`.
- `tarea-2`: `Presentation/`, `.gitignore`, `src.sln`.

El proyecto Help Desk se desarrolla en:

```text
C:\Users\Diego\OneDrive\Documents\Uned\Programación Avanzada en WEB\Proyectos\proyecto-1
```

La solución de Visual Studio está en:

```text
C:\Users\Diego\OneDrive\Documents\Uned\Programación Avanzada en WEB\Proyectos\proyecto-1\src
```

## Problema

Al principio se intentó trabajar desde el repositorio padre:

```text
C:\Users\Diego\OneDrive\Documents\Uned\Programación Avanzada en WEB\Proyectos
```

Ese repositorio ya contenía las ramas del curso y además tenía una entrada en `.git/info/exclude`:

```text
proyecto-1/
```

Por eso Git ignoraba todo el contenido de `proyecto-1/`. Si se quitaba esa exclusión, Git trataba la carpeta como un repositorio anidado o como un posible submódulo, y no versionaba los archivos internos de `src` de la forma esperada.

Además, la rama remota `proyecto-1` existía como un commit vacío:

```text
a9e28dc
```

Ese commit no tenía archivos; era solo el punto de partida de la rama.

## Diagnóstico

Comandos usados para entender la situación:

```powershell
git rev-parse --show-toplevel
git branch -a
git log --oneline --graph --all
git check-ignore -v proyecto-1
git ls-tree --name-only origin/proyecto-1
```

Hallazgos:

- La raíz del repositorio padre era `Proyectos`.
- `proyecto-1/` estaba excluido mediante `.git/info/exclude`.
- La rama remota `proyecto-1` no contenía archivos del proyecto.
- El proyecto real estaba un nivel más abajo, en `proyecto-1\src`.
- Si se inicializaba Git en `proyecto-1`, los archivos quedarían como `src/App_Start/...`, distinto del patrón del curso.

## Solución

Se creó un repositorio independiente en la carpeta que representa la raíz del proyecto dentro de la rama:

```text
...\proyecto-1\src
```

Así, los archivos quedan directamente en la raíz de la rama:

```text
App_Start/
Content/
Controllers/
Models/
Views/
Web.config
Help-Desk.sln
HelpDesk.Web.csproj
```

y no como:

```text
proyecto-1/src/App_Start/...
```

Comandos ejecutados desde `...\proyecto-1\src`:

```powershell
git init
git remote add origin https://github.com/Dicocha/Programacion-Avanzada-Web.git
git fetch origin
git checkout -b proyecto-1 origin/proyecto-1
```

Luego se agregó la estructura inicial:

```powershell
git add .
git status
git commit -m "chore: estructura inicial del proyecto Help Desk MVC5"
git push -u origin proyecto-1
```

## Exclusión del repositorio padre

En el repositorio padre se mantiene la línea de `.git/info/exclude`:

```text
C:\Users\Diego\OneDrive\Documents\Uned\Programación Avanzada en WEB\Proyectos\.git\info\exclude
```

Contenido relevante:

```text
proyecto-1/
```

Esto evita que el repositorio padre intente versionar el repositorio interno como si fuera un submódulo o una carpeta normal.


## Verificación

Comandos para comprobar que la rama quedó correcta:

```powershell
git rev-parse --show-toplevel
git status
git log --oneline --graph --all
git ls-tree --name-only origin/proyecto-1
```

Resultado esperado:

- `git rev-parse --show-toplevel` devuelve `...\proyecto-1\src`.
- `git status` no muestra `bin`, `obj`, `packages`, `Backup` ni `UpgradeLog.htm`.
- `git ls-tree --name-only origin/proyecto-1` muestra `App_Start/`, `Controllers/`, `Views/`, `Web.config`, etc. en la raíz.
- La rama local `proyecto-1` sigue a `origin/proyecto-1`.

## Lecciones

1. La raíz del repositorio debe coincidir con la raíz esperada dentro de la rama.
2. Un repositorio Git dentro de otro repositorio Git se trata como repositorio anidado o submódulo; no se deben mezclar sin una estrategia explícita.
3. `.git/info/exclude` es local y no se versiona; sirve para ignorar carpetas sin modificar `.gitignore`.
4. Antes de hacer `push`, conviene revisar `git status`, `git log` y el árbol de la rama remota.
5. Los artefactos de Visual Studio y los paquetes restaurados no deben entrar al repositorio.
6. Cada funcionalidad debe tener su propio commit para mantener un historial claro.
