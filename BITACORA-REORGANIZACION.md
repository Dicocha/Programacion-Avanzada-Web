# Bitácora de la reorganización del repositorio

**Proyecto:** Programación Avanzada en Web (03101) — UNED
**Fecha:** 21 de octubre de 2026
**Repositorio:** `https://github.com/Dicocha/Programacion-Avanzada-Web.git`
**Objetivo:** reemplazar el historial mezclado del repositorio por 5 ramas
limpias, una por entrega, con un commit por funcionalidad.

> Este documento es la contraparte práctica de `GUIA-EJECUCION.md`:
> la guía explica **cómo ejecutar** los scripts; esta bitácora explica
> **qué se hizo realmente**, con los comandos literales, los errores que
> salieron y cómo se diagnosticaron.

---

## 1. Punto de partida

La situación original era esta:

- **Dos repositorios en GitHub.** Uno (`Programacion-Avanzada-Web`) con
  `master` y `tarea-2`; otro, independiente, con la Tarea 2.
- **Historial mezclado.** `master` y `tarea-2` apuntaban al *mismo* commit
  (`8c9b556`), que contenía la Tarea 1 con 9 commits (8 de funcionalidad +
  "Add README.md") y el código **en la raíz** del repositorio, no en
  `tarea-1/src/`.
- **Rama `tarea-2` engañosa.** Por nombre parecía contener la Tarea 2, pero
  su contenido era el de la Tarea 1.
- **Un solo clon local**, con las dos tareas en carpetas (`tarea-1/`,
  `tarea-2/`) y el código viejo movido a la raíz.

Medición inicial del remoto (solo lectura, no modifica nada):

```sh
$ git ls-remote --heads origin
8c9b5561a82fa91ed05a0419670ed135e58284a0        refs/heads/master
8c9b5561a82fa91ed05a0419670ed135e58284a0        refs/heads/tarea-2
```

Ese `8c9b556` es el commit **viejo**. En el §7 está el estado final.

---

## 2. Diseño final

| Rama         | Contenido                                        | Commits | Hash      |
| ------------ | ------------------------------------------------ | ------- | --------- |
| `master`     | `README.md` + esta bitácora                      | 2       | ver §7    |
| `tarea-1`    | `tarea-1/src/` — Web Forms en 4 capas             | 8       | `40ec1b6` |
| `tarea-2`    | `tarea-2/src/` — ASP.NET Core MVC (To-Do)         | 6       | `fb5d8ac` |
| `proyecto-1` | commit inicial vacío (esperando enunciado)       | 1       | `a9e28dc` |
| `proyecto-2` | commit inicial vacío (esperando enunciado)       | 1       | `6d7917c` |

Reglas que se aplicaron:

1. **Un commit = una funcionalidad.** El historial se lee de arriba abajo y
   cuenta la historia del desarrollo.
2. **Cada rama contiene solo lo suyo.** Nada de `bin/`, `obj/`, `packages/`,
   `.vs/`, videos ni documentos técnicos.
3. **El último commit de cada rama = el código original**, byte por byte.

---

## 3. Las herramientas

Todo vive en `herramientas/`, fuera del versionado.

| Archivo                 | Función                                                                      |
| ----------------------- | ---------------------------------------------------------------------------- |
| `reorganizar-repo.sh`   | Orquestador: construye las 5 ramas en un temporal, verifica y publica.       |
| `publicar.sh`           | Publica las 5 ramas leyendo los commits desde el bundle.                     |
| `sincronizar-local.sh`  | Alinea el clon local con lo publicado (`fetch` + `reset` + `branch -f`).      |
| `tarea-1-commits.sh`    | Genera los 8 commits de la Tarea 1 (estrategia "por rutas").                 |
| `tarea-2-commits.sh`    | Genera los 6 commits de la Tarea 2 (estrategia "re-escribiendo").           |
| `ramas-limpias.bundle`  | Las 5 ramas ya construidas y verificadas, empaquetadas (1.1 MB).              |

**El bundle** es la pieza clave: un archivo que git puede usar como si fuera
un servidor. `git bundle list-heads` lo lee sin tocar nada:

```sh
$ git bundle list-heads herramientas/ramas-limpias.bundle
6585c13749635ebc4709d42fcd22736dff6655fb refs/heads/master
a9e28dcaf3251732bd5db9de3c514c7e57982417 refs/heads/proyecto-1
6d7917cd804e0d1d55a87ba5a9ebb372b13cdd3f refs/heads/proyecto-2
40ec1b6b37df8be3e8c9c4f3a58daa46e2716a3a refs/heads/tarea-1
fb5d8ac42e0de4a66ba6412ebf9bf37b7a8a4e3e refs/heads/tarea-2
```

> Aclaración importante: el bundle **no** se creó durante esta sesión, ya
> existía en el entorno. `paquetes` tampoco es el nombre del bundle: es el
> nombre que se le dio a un *remoto* que apunta al archivo:
>
> ```sh
> git remote add paquetes "$(pwd)/herramientas/ramas-limpias.bundle"
> ```
>
> Es decir: `ramas-limpias.bundle` = el archivo; `paquetes` = el alias con
> el que git lo conoce; `origin` = GitHub. El bundle fue el **origen** de
> los commits, GitHub el **destino**.

---

## 4. Procedimiento ejecutado

### Fase 0 — Preparación

```sh
git remote -v                      # confirmar el remoto
git config --local user.name "Diego Coto"
git config --local user.email "diegoesteban.coto@uned.cr"
git branch respaldo/master master  # red de seguridad local
```

La identidad coincidía con la usada al construir el bundle, lo que permitió
usar la vía rápida (publicar desde el bundle) sin reconstruir nada.

### Fase 1 — Traer las 5 ramas desde el bundle

```sh
git remote add paquetes "$(pwd)/herramientas/ramas-limpias.bundle"
git fetch --force paquetes
git branch -a | grep paquetes
```

Se crea un remoto *temporal* llamado `paquetes` que apunta al archivo
bundle, y se descargan las 5 ramas como referencias remotas
(`refs/remotes/paquetes/*`). En este punto **GitHub sigue intacto**.

### Fase 2 — Publicar con `push --force`

```sh
for b in tarea-1 tarea-2 proyecto-1 proyecto-2; do
  git push --force origin "paquetes/$b:refs/heads/$b"
done
```

El **refspec** tiene dos lados separados por `:` — *origen:destino*:

```
paquetes/tarea-1        :        refs/heads/tarea-1
        ↑                          ↑
   DE DÓNDE SACO               A DÓNDE LO PONGO
```

- **Lado izquierdo:** `paquetes/tarea-1` es la forma abreviada de
  `refs/remotes/paquetes/tarea-1`, la referencia local que creó el `fetch`
  del bundle.
- **Lado derecho:** `refs/heads/tarea-1` es el nombre interno completo de
  una rama (toda rama es una ref bajo `refs/heads/`).

Sin el `:` ya valdría `git push origin tarea-1` (origen y destino con el
mismo nombre). El refspec explícito permite **emparejar nombres distintos**,
que es el caso aquí: la rama del bundle → la rama homónima en GitHub.

`--force` es indispensable: el remoto ya tenía un historial y sin esta
opción git rechazaría el push por no ser _fast-forward_.

> Antes del push real se hizo un **ensayo con `--dry-run`**, que muestra qué
> ocurriría sin escribir nada. Los scripts no lo incluyen: es una precaución
> añadida a mano, y conviene repetirla siempre antes de un `push --force`.

### Fase 3 — Sincronizar el clon local

```sh
sh herramientas/sincronizar-local.sh
```

Que internamente hace:

1. `git fetch --force origin` — descarga las referencias nuevas.
2. Verifica que `origin/master` contenga solo `README.md`.
3. **Guardia de seguridad**: aborta si detecta `Domain/`, `Application/`,
   `Infrastructure/`, `Presentation/`, `src.sln` o `Tarea 1.slnx` en la
   raíz, porque el paso siguiente los borraría del disco.
4. `git reset --hard origin/master` — mueve `master` y **sobrescribe** la
   carpeta para que coincida con el commit (dejando el README y retirando
   el `.gitignore` viejo de la raíz).
5. `git branch -f <rama> origin/<rama>` × 4 — re-apunta `tarea-1` y crea
   `tarea-2`, `proyecto-1` y `proyecto-2` en local, sin cambiar de rama.
6. Escribe en `.git/info/exclude` las carpetas de trabajo y las
   herramientas, para que no aparezcan como "untracked".

### Fase 4 — Verificación

```sh
git ls-remote --heads origin     # 5 ramas, 5 hashes distintos
git log --oneline origin/tarea-1 # 8 commits
git log --oneline origin/tarea-2 # 6 commits
git status                       # working tree clean
```

---

## 5. Conceptos de git aplicados

| Concepto                             | Dónde apareció                                  | Para qué sirvió |
| ------------------------------------ | ----------------------------------------------- | --------------- |
| **refspec `origen:destino`**        | `git push origin "paquetes/X:refs/heads/X"`    | Elegir qué rama local va a qué rama remota. |
| **`--force`**                        | Los 5 pushes                                    | Reemplazar un historial remoto existente (reescribir historia). |
| **`--dry-run`**                     | Antes del push                                  | Ver qué haría sin escribir nada. Salvó un error (§6.1). |
| **bundle como remoto**               | `git remote add paquetes <archivo>`             | Reutilizar historia ya verificada sin reconstruirla. |
| **remote-tracking refs**             | `refs/remotes/paquetes/*`, `origin/*`           | Espejo local del remoto; permite comparar antes de empujar. |
| **`reset --hard`**                   | Fase 3                                          | Alinear rama y carpeta con un commit. **Descarta cambios locales.** |
| **`branch -f`**                      | Fase 3                                          | Mover una rama sin checkout (no toca los archivos). |
| **`symbolic-ref HEAD`**              | En la construcción de las ramas                 | Nombrar la rama antes del primer commit (evita `main`). |
| **`commit --allow-empty`**           | Ramas `proyecto-*`                              | Crear la rama con un commit inicial sin archivos. |
| **`.gitignore` vs `.git/info/exclude`** | Carpetas de trabajo                          | El primero se versiona; el segundo es local y no viaja al remoto. |
| **`ls-remote` / `list-heads`**       | Verificaciones                                   | Ver el estado real del remoto y del bundle sin modificar nada. |
| **`history`**                        | Diagnóstico (§6.1)                               | Verificar qué comandos se ejecutaron de verdad. |

---

## 6. Incidentes y diagnóstico

### 6.1 Las 5 ramas apuntaban al mismo commit

**Síntoma:** tras los primeros pushes, `git ls-remote --heads origin`
devolvía las 5 ramas con el mismo hash (`6585c137`, el de `master`).

**Diagnóstico:**

```sh
git bundle list-heads herramientas/ramas-limpias.bundle  # el bundle: 5 hashes distintos
git log --oneline refs/remotes/paquetes/tarea-1          # local: 8 commits, correcto
history | tail -15                                       # <- el culpable
```

El historial mostró la causa: en las 4 órdenes de push, la parte izquierda
del refspec se había quedado en `paquetes/master`:

```
git push --force origin "paquetes/master:refs/heads/tarea-1"
```

Un desliz de copy/paste al seleccionar el bloque en la terminal. **Git hizo
exactamente lo que se le pidió**: si el origen dice `master`, se sube
`master`. Git no adivina ni advierte.

**Corrección:** usar un bucle, de modo que origen y destino usen la misma
variable y no puedan desincronizarse, y **ensayar primero**:

```sh
for b in tarea-1 tarea-2 proyecto-1 proyecto-2; do
  git push --force --dry-run origin "paquetes/$b:refs/heads/$b"
done
```

El ensayo mostró 4 hash distintos → luego el push real.

**Lección:** antes de un `push --force`, un `--dry-run` y un `git ls-remote`
evitan sorpresas. Y `history | tail -15` es el mejor detective cuando
algo no sale como se espera.

### 6.2 PowerShell no ejecuta los scripts

Los scripts son `#!/usr/bin/env sh`. En PowerShell fallaron; la solución
fue usar **Git Bash** (MINGW64), que trae `sh` y `git` ya instalados.

### 6.3 El repositorio dentro de OneDrive

El clon vive en `C:\Users\Diego\OneDrive\Documents\...`. OneDrive
sincroniza en segundo plano y `reset --hard` (que borra y reescribe
archivos masivamente) es el peor escenario posible para eso. Recomendación:
trabajar fuera de OneDrive, o al menos tener en cuenta que la primera
sincronización puede tardar o dar conflictos.

### 6.4 `$ffor` → variable inexistente

La primera línea del bucle se pegó como `$ffor b in ...`. En bash, `$ffor`
es una variable no definida y se expande a cadena vacía, dejando
` for b in ...` — un comando válido. Funcionó por accidente: no conviene
confiar en ello.

---

## 7. Estado final

```sh
$ git branch -vv
* master          6585c13 [origin/master]        docs: README ...
  proyecto-1      a9e28dc [origin/proyecto-1]     chore: inicializa proyecto-1 ...
  proyecto-2      6d7917cd [origin/proyecto-2]     chore: inicializa proyecto-2 ...
  tarea-1         40ec1b6 [origin/tarea-1]        feat(presentation): validaciones cliente ...
  tarea-2         fb5d8ac [origin/tarea-2]        feat: filtrar tareas por estado ...
```

> El hash que se muestra para `master` es el del commit con el README.
> Al versionar esta bitácora, `master` avanza a un commit nuevo (queda con
> 2 commits: el README y este documento).

- El clon local quedó **limpio**: `git status` → *working tree clean*.
- Las carpetas de trabajo (`tarea-1/`, `tarea-2/`, `proyecto-1/`,
  `proyecto-2/`, `herramientas/`, `GUIA-EJECUCION.md`) siguen en el disco
  pero quedan fuera del control de versiones.
- La rama `respaldo/master` conserva el historial viejo **solo en local**
  (nunca se sube); sirve de red de seguridad.

---

## 8. Qué se borró y qué no

| Qué                                         | ¿Borrado? | Detalle |
| ------------------------------------------- | --------- | ------- |
| Historial visible de `master`                | ✅ Sí     | Sustituido por el commit con el README. |
| Historial visible de `tarea-2`               | ✅ Sí     | Sustituido por los 6 commits de la Tarea 2. |
| Commits antiguos "colgados"                  | ❌ No     | Siguen accesibles por su SHA en GitHub ~90 días. |
| Historial en otros clones o forks propios     | ❌ No     | Sobrevive para siempre donde exista una copia. |

Para una purga total (que también elimina los objetos colgados) la única
vía es **borrar el repositorio y recrearlo con el mismo nombre**, y
volver a subir el bundle. Eso se puede hacer más adelante sin perder nada,
porque el bundle sigue en el disco.

**Pendiente por autorización:** el repositorio de GitHub independiente que
contiene la Tarea 2 se conserva hasta que el docente autorice eliminarlo.
Su código ya está a salvo en la rama `tarea-2` del repositorio principal.

---

## 9. Cheatsheet

```sh
# Estado
git status                     # qué hay sin confirmar
git log --oneline             # historial resumido
git branch -vv                 # ramas locales + a qué remoto apuntan
git ls-remote --heads origin   # ramas reales en GitHub (solo lectura)

# Antes de publicar
git push --force --dry-run origin "paquetes/X:refs/heads/X"

# Publicar
git push --force origin "paquetes/X:refs/heads/X"
git push origin --delete <rama>       # borrar una rama del remoto
git remote remove paquetes            # quitar el remoto temporal

# Sincronizar
git fetch --force origin
git reset --hard origin/master
git branch -f <rama> origin/<rama>

# Diagnóstico
git bundle list-heads <archivo.bundle>
git reflog
history | tail -15
```

---

## 10. Cómo seguir trabajando

```sh
git checkout tarea-2              # cambiar a la rama de trabajo
git status                        # ver qué hay
git add -A && git commit -m "..."  # un commit por funcionalidad
git push origin tarea-2           # subir (sin --force: es avance lineal)
```

Para empezar un proyecto nuevo:

```sh
git checkout proyecto-1
# ... escribir el código ...
git add -A && git commit -m "feat: estructura inicial del proyecto"
git push origin proyecto-1
```

Las ramas `proyecto-1` y `proyecto-2` ya existen en el remoto con un
commit vacío, listas para recibir el código del enunciado.
