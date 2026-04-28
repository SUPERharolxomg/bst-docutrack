# 📄 ArbolView.cs — Explicación detallada

## ¿Qué es este archivo?

Es la **pantalla del sistema**. Es la única clase que tiene permiso de escribir en la consola con `Console.Write` y `Console.WriteLine`. Recibe datos del Controller y los presenta de forma visual. No calcula nada, no toma decisiones, solo muestra.

> ✅ **Regla MVC:** Si algo aparece en pantalla, pasó por aquí. Si algo se calcula, no pasó por aquí.

---

## Cabecera — los `using`

```csharp
using System;                      // para Console y ConsoleColor
using System.Collections.Generic;  // para List<Nodo> en los recorridos
using DocuTrack.Model;             // para conocer el tipo Nodo
```

`System` → necesario para `Console`, `ConsoleColor`.
`System.Collections.Generic` → necesario para recibir `List<Nodo>` como parámetro.
`DocuTrack.Model` → necesario para trabajar con objetos `Nodo`. La View los recibe como datos pero no los modifica.

---

## MostrarBanner

```csharp
Console.ForegroundColor = ConsoleColor.Magenta;
```

**¿Qué hace?** Cambia el color del texto de la consola a **magenta** (violeta). Todo lo que se imprima **después** de esta línea saldrá en ese color, hasta que se resetee.

```csharp
Console.WriteLine("  ╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("  ║        DocuTrack S.A. — Gestión Jerárquica de Documentos    ║");
Console.WriteLine("  ║            Árbol Binario de Búsqueda  (BST)  — MVC C#       ║");
Console.WriteLine("  ╚══════════════════════════════════════════════════════════════╝");
```

Imprime el marco decorativo del banner. Los caracteres `╔`, `═`, `╗`, `║`, `╚`, `╝` son caracteres Unicode de caja. Son texto normal, no dibujos especiales.

```csharp
Console.ResetColor();
```

**¿Por qué es importante?** Devuelve el color de la consola al predeterminado. **Siempre debe llamarse** después de cambiar el color. Si se olvida, todo el texto que siga saldrá en magenta.

`MostrarFin()` hace lo mismo pero imprime la línea de `═` al final de la ejecución.

---

## MostrarEncabezado

```csharp
Console.WriteLine(new string('═', 65));
Console.WriteLine($"  ▶  {titulo}");
Console.WriteLine(new string('═', 65));
```

**`new string('═', 65)`** → constructor de `string` que crea una cadena repitiendo el carácter `'═'` exactamente 65 veces. Resultado: `═════════════════════════════════════════════════════════════════`.

**`$"  ▶  {titulo}"`** → interpolación de strings. El `$` al inicio activa esta función. `{titulo}` se reemplaza por el valor de la variable `titulo`. Por ejemplo, si `titulo = "CASO 1"`, imprime `  ▶  CASO 1`.

---

## MostrarSubtitulo

```csharp
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine($"  ┌─ {texto}");
Console.ResetColor();
```

Imprime en **cyan** (azul claro) con el símbolo `┌─` al inicio. Se usa para los subtítulos dentro de cada caso de uso. Siempre resetea el color al final.

---

## ImprimirArbol — el método más importante de la View

```csharp
public void ImprimirArbol(Nodo? raiz)
```

**¿Qué recibe?** La raíz del árbol. El Controller lo llama así: `_vista.ImprimirArbol(_arbol.Raiz)`.

```csharp
if (raiz == null) { Console.WriteLine("  (árbol vacío)"); Console.WriteLine(); return; }
```

**Caso especial:** Si el árbol está vacío (raíz es `null`), imprime un mensaje y sale. El `return` evita que el resto del método se ejecute.

```csharp
ImprimirNodoRec(raiz, "  ", esUltimo: true);
```

**¿Qué hace?** Llama al método recursivo que realmente dibuja el árbol, pasando:
- `raiz` → el primer nodo a imprimir.
- `"  "` → la sangría inicial (2 espacios).
- `esUltimo: true` → la raíz no tiene hermanos, siempre es "el último" de su nivel.

---

### ImprimirNodoRec — cómo se construye el árbol ASCII

```csharp
string conector    = esUltimo ? "└── " : "├── ";
string continuador = esUltimo ? "    " : "│   ";
```

**`esUltimo`** determina dos cosas: qué conector usar y si la rama vertical sigue hacia abajo.

- Cuando `esUltimo = true` (último hijo de su padre):
  - Conector: `└──` (la rama termina aquí, dobla hacia la derecha).
  - Continuador: `    ` (4 espacios; no hay línea vertical porque la rama ya terminó).

- Cuando `esUltimo = false` (hay más hermanos debajo):
  - Conector: `├──` (la rama continúa hacia abajo y también dobla a la derecha).
  - Continuador: `│   ` (línea vertical + 3 espacios; la rama sigue y los hijos deben alinearse).

```csharp
Console.ForegroundColor = nodo.EsCarpeta ? ConsoleColor.Yellow : ConsoleColor.White;
Console.WriteLine(prefijo + conector + nodo.Icono + " " + nodo.Nombre);
Console.ResetColor();
```

**Construye la línea completa** concatenando:
1. `prefijo` → la sangría acumulada de los niveles superiores.
2. `conector` → el `└──` o `├──`.
3. `nodo.Icono` → `[DIR] ` o `[FILE]`.
4. `" "` → un espacio separador.
5. `nodo.Nombre` → el nombre del nodo.

**Color:** amarillo para carpetas, blanco para archivos.

**Ejemplo de línea resultante:**
```
    ├── [DIR]  Facturas
```

```csharp
string nuevoPrefijo = prefijo + continuador;
```

**El corazón del algoritmo.** El prefijo para los hijos es el prefijo actual más el continuador. Así, cuando se imprimen los hijos del hijo, la sangría lleva acumuladas todas las ramas verticales de los niveles superiores.

**Ejemplo de cómo se acumula:**
```
Nivel 0: prefijo=""     → "  └── Proyectos"
Nivel 1: prefijo="      " → "      ├── Facturas"
Nivel 2: prefijo="      │   " → "      │   └── Contratos"
```

```csharp
if (tieneIzq && tieneDer)
{
    ImprimirNodoRec(nodo.Izquierdo!, nuevoPrefijo, esUltimo: false);
    ImprimirNodoRec(nodo.Derecho!,   nuevoPrefijo, esUltimo: true);
}
else if (tieneIzq) ImprimirNodoRec(nodo.Izquierdo!, nuevoPrefijo, esUltimo: true);
else if (tieneDer) ImprimirNodoRec(nodo.Derecho!,   nuevoPrefijo, esUltimo: true);
```

**Si tiene los dos hijos:**
- El izquierdo se imprime con `esUltimo: false` porque el derecho vendrá después.
- El derecho se imprime con `esUltimo: true` porque es el último.

**Si tiene solo uno:** ese hijo es el único → siempre `esUltimo: true`.

**El `!` al final** (`nodo.Izquierdo!`) es el operador **null-forgiving**: le dice al compilador "sé que esto no es null en este punto, aunque el tipo lo permita". Sin él, el compilador advertiría un posible null.

---

## MostrarInsercion

```csharp
if (insertado)
    Console.WriteLine($"    ✓  Insertado : '{nombre}'  |  comparaciones: {comparaciones}");
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"    ✗  Duplicado rechazado: '{nombre}' ya existe.");
    Console.ResetColor();
}
```

**Si se insertó** → imprime ✓ en color normal con el nombre y las comparaciones usadas.
**Si era duplicado** → cambia el color a rojo, imprime ✗ con el mensaje, y resetea el color.

---

## MostrarResultadoBusqueda

```csharp
Console.Write($"    Buscar('{nombre}')  →  ");
```

**`Console.Write`** (sin `Line`) → imprime el texto **sin agregar salto de línea**. El resultado de la búsqueda (`HALLADO` o `NO ENCONTRADO`) quedará en la misma línea, a continuación.

```csharp
if (nodo != null)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"HALLADO  {nodo.Icono} '{nodo.Nombre}'  |  comparaciones: {comparaciones}");
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"NO ENCONTRADO  |  comparaciones: {comparaciones}");
}
Console.ResetColor();
```

**Si `nodo != null`** → se encontró → verde con los datos del nodo hallado.
**Si `nodo == null`** → no existe → rojo.
Las comparaciones siempre se muestran en ambos casos.

---

## MostrarActualizacion / MostrarEliminacion

Ambos siguen el mismo patrón:
- Si la operación tuvo **éxito** → verde ✓ + detalle de qué cambió.
- Si **falló** → rojo ✗ + mensaje de error.
- `Console.ResetColor()` siempre al final.

En `MostrarEliminacion`, el campo `caso` contiene el texto que generó `ArbolBinario` describiendo cuál de los 3 casos BST se ejecutó. Eso es lo que aparece como "Caso 1", "Caso 2" o "Caso 3" en consola.

---

## MostrarRecorrido

```csharp
Console.Write($"    {tipo,-12}: ");
```

**`{tipo,-12}`** → alineación a la izquierda en un campo de mínimo 12 caracteres. Como todos los recorridos usan el mismo formato, esto hace que los `:` queden alineados en columna:
```
    Preorden    : ...
    Inorden     : ...
    Postorden   : ...
    Por Niveles : ...
```

```csharp
foreach (Nodo n in nodos)
    Console.Write($"{n.Nombre}({(n.EsCarpeta ? "D" : "F")})  ");
Console.WriteLine();
```

**`foreach`** → recorre la lista en el orden que entregó el recorrido (ya ordenada por el Model).
Imprime cada nodo como `Nombre(D)` si es carpeta o `Nombre(F)` si es archivo.
`Console.WriteLine()` al final agrega el salto de línea que `Console.Write` no pone.

---

## MostrarAltura

```csharp
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine($"    Altura del árbol final: {altura} nivel(es)");
Console.ResetColor();
```

Imprime en cyan el número de niveles del árbol, calculado por `ArbolBinario.Altura()`. La View solo lo recibe e imprime; no lo calcula.

---

## MostrarMensaje

```csharp
public void MostrarMensaje(string mensaje) => Console.WriteLine($"    {mensaje}");
```

Método de propósito general. El `=>` es una **expresión de cuerpo** (forma corta de `{ Console.WriteLine(...); }`). Imprime cualquier texto plano con 4 espacios de sangría al inicio.
