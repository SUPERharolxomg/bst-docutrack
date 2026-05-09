# DocuTrack BST

Sistema de gestión jerárquica de documentos implementado como 
Árbol Binario de Búsqueda (BST) en C# bajo arquitectura MVC.

Proyecto académico — Programación III  
Universidad de Manizales · Facultad de Ciencias e Ingeniería

---

## Integrantes

| Nombre | Responsabilidad |
|--------|----------------|

---

## Requisitos para ejecutar

- [.NET SDK 8.0](https://dotnet.microsoft.com/download) o superior
- Sistema operativo: Windows, Linux o macOS
- Terminal o consola con soporte UTF-8 (para los conectores ASCII)

---

## Cómo clonar y ejecutar

```bash
# 1. Clonar el repositorio
git clone https://github.com/usuario/docutrack-bst.git

# 2. Entrar al proyecto
cd docutrack-bst/DocuTrack

# 3. Compilar
dotnet build

# 4. Ejecutar
dotnet run
```

---

## Arquitectura del proyecto
```
DocuTrack/
├── Model/
  ├── Nodo.cs # Clase nodo: Nombre, EsCarpeta, Izquierdo, Derecho
  └── ArbolBinario.cs # CRUD, recorridos y altura. Sin Console.
├── View/
  └── ArbolView.cs # Impresión ASCII, resultados y métricas
├── Controller/
  └── ArbolController.cs # Orquestación de los 6 casos de uso
  └── Program.cs # Punto de entrada. Solo invoca Controller.
```
| Capa | Responsabilidad | Restricción |
|------|----------------|-------------|
| Model | Lógica BST, comparaciones, nodos | Sin `Console` |
| View | Solo `Console.Write/WriteLine` | Sin lógica |
| Controller | Secuencia de casos de uso | Sin lógica BST |

---

## Estructura del BST

Cada nodo representa una **carpeta** o un **archivo** del sistema DocuTrack:

- La clave de ordenamiento es el **Nombre** (comparación ordinal, ignore-case)
- Un **archivo** es siempre nodo hoja
- Una **carpeta** puede tener 0, 1 o 2 hijos
- No se permiten nombres duplicados

---

## Casos de uso implementados

1. **Construcción** — inserción de 14 nodos iniciales (carpetas y archivos)
2. **Búsquedas** — 6 búsquedas con contador de comparaciones
3. **Actualizaciones** — 3 actualizaciones (nodo hoja, un hijo, raíz)
4. **Eliminaciones** — 3 eliminaciones (nodo hoja, un hijo, raíz)
5. **Recorridos** — Preorden, Inorden, Postorden y Por niveles
6. **Altura** — cálculo y muestra al finalizar

---

## ¿Por qué el Inorden confirma el BST?

El recorrido **Inorden** (izquierdo → raíz → derecho) visita los nodos
en orden ascendente según la clave. Si la secuencia resultante está
**estrictamente ordenada de menor a mayor** (alfabéticamente),
se confirma que la propiedad del BST se mantiene intacta tras
cualquier operación de inserción, actualización o eliminación.

Los recorridos Preorden y Postorden no garantizan este ordenamiento
por sí solos, por eso Inorden es el recorrido de verificación estándar
para un BST.

---

## Ejemplo de salida en consola
```
══════════════════════════════════════
CASO 1 — Árbol inicial (14 nodos)
══════════════════════════════════════
proyectos [C]
├── contratos [C]
│ ├── archivo_2022.pdf [A]
│ └── facturacion [C]
└── reportes [C]
└── resumen_anual.xlsx [A]
══════════════════════════════════════
CASO 2 — Búsquedas (6 total)
══════════════════════════════════════
[ENCONTRADO] "contratos" — comparaciones: 2 (subárbol izquierdo)
[ENCONTRADO] "archivo_2022.pdf" — comparaciones: 3 (subárbol izquierdo)
[ENCONTRADO] "reportes" — comparaciones: 2 (subárbol derecho)
[ENCONTRADO] "resumen_anual.xlsx" — comparaciones: 3 (subárbol derecho)
[NO ENCONTRADO] "ventas" — comparaciones: 2
[NO ENCONTRADO] "nomina.xls" — comparaciones: 3
══════════════════════════════════════
CASO 5 — Eliminaciones
══════════════════════════════════════
Eliminando "archivo_2022.pdf" → caso: HOJA
Eliminando "facturacion" → caso: UN HIJO
Eliminando "proyectos" → caso: DOS HIJOS (sucesor: reportes)
══════════════════════════════════════
CASO 6 — Recorridos y altura final
══════════════════════════════════════
Preorden : proyectos, contratos, reportes ...
Inorden : archivo_2022, contratos, facturacion ...
Postorden : archivo_2022, facturacion, contratos ...
Por niveles: proyectos | contratos, reportes | ...
Altura del árbol: 4
```

## Release

El commit final de entrega está etiquetado como `release-unidad1`.

```bash
git checkout tags/release-unidad1
```
