# 📄 Program.cs — Explicación detallada

## ¿Qué es este archivo?

Es el **punto de arranque** de toda la aplicación. Cuando ejecutas `dotnet run`, lo primero que hace .NET es correr este archivo. Su trabajo es mínimo: preparar la consola y llamar al Controller para que haga todo lo demás.

> ⚠️ **Regla del enunciado:** `Program.cs` solo puede invocar al Controller. No puede tener lógica de negocio, cálculos, ni `Console.Write` propios.

---

## El código, explicado parte por parte

### 1. Importar el Controller

```csharp
using DocuTrack.Controller;
```

**¿Qué hace?** Le dice al compilador que este archivo va a usar clases del espacio de nombres `DocuTrack.Controller`. Sin esta línea, el compilador no reconocería el nombre `ArbolController` más abajo y daría error.

---

### 2. Configurar el encoding de la consola

```csharp
System.Console.OutputEncoding = System.Text.Encoding.UTF8;
```

**¿Qué hace?** Configura la consola para que use la codificación de caracteres **UTF-8**.

**¿Por qué es necesario?** El árbol ASCII usa caracteres especiales que solo existen en UTF-8. Sin esta línea se verían así:

| Con UTF-8 | Sin UTF-8 |
|-----------|-----------|
| `╔══════╗` | `????` |
| `│ ├── └──` | `? ?-- ?--` |
| `✓ ✗` | `? ?` |

**¿Por qué `System.Console` con el prefijo largo?** Porque no hay `using System;` al inicio del archivo. Se usa el nombre completo para evitar el `using` extra.

**¿Por qué debe ser la primera línea?** Porque si se imprime algo antes de configurar el encoding, esas líneas ya saldrán con la codificación incorrecta.

---

### 3. Crear el Controller

```csharp
var controller = new ArbolController();
```

**¿Qué hace?** Crea una nueva instancia de `ArbolController`.

**¿Qué pasa dentro de `ArbolController` cuando se crea?** Su constructor crea internamente:
- Un `ArbolBinario` (el árbol vacío, listo para recibir nodos).
- Un `ArbolView` (la pantalla, lista para imprimir).

**¿Qué significa `var`?** El compilador detecta automáticamente el tipo del objeto. Es equivalente a escribir `ArbolController controller = new ArbolController()`, pero más corto.

---

### 4. Ejecutar todo

```csharp
controller.Ejecutar();
```

**¿Qué hace?** Llama al método `Ejecutar()` del Controller, que a su vez ejecuta **todos los casos de uso** en orden:
1. Banner de bienvenida
2. Construcción del árbol (14 nodos)
3. 6 búsquedas con comparaciones
4. 3 actualizaciones con impresión del árbol
5. 3 eliminaciones con impresión del árbol
6. 4 recorridos (Pre, In, Post, Por Niveles)
7. Altura final del árbol
8. Pie de página

Todo el trabajo ocurre dentro de esta sola línea.

---

## ¿Por qué el archivo es tan corto?

Porque el enunciado exige que `Program.cs` no tenga nada más que la llamada al Controller. Esta es la regla del patrón MVC: el punto de entrada no decide nada, no muestra nada, no calcula nada. Solo enciende el motor y se aparta.
