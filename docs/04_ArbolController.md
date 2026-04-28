# 📄 ArbolController.cs — Explicación detallada

## ¿Qué es este archivo?

Es el **director de orquesta**. Su único trabajo es llamar a los métodos correctos en el orden correcto. No inventa lógica del árbol, no calcula nada, no imprime nada directamente. Todo lo delega: la lógica va al Model (`ArbolBinario`), la presentación va a la View (`ArbolView`).

> ⚠️ **Regla MVC:** El Controller no tiene `Console.Write` ni lógica de BST. Solo coordina.

---

## Campos y constructor

```csharp
private readonly ArbolBinario _arbol;
private readonly ArbolView    _vista;
```

**`private`** → estas variables solo existen y son accesibles dentro de esta clase.

**`readonly`** → solo se pueden asignar **dentro del constructor**. Una vez creadas, no pueden apuntar a otro objeto. Esto garantiza que el Controller siempre trabaje con el mismo árbol y la misma vista durante toda la ejecución.

**Convención de nombres:** El guion bajo `_` al inicio (`_arbol`, `_vista`) es la convención para campos privados en C#. Diferencia visualmente un campo privado de una variable local.

```csharp
public ArbolController()
{
    _arbol = new ArbolBinario();
    _vista = new ArbolView();
}
```

**¿Qué hace?** Cuando `Program.cs` escribe `new ArbolController()`, este constructor se ejecuta y crea:
- Un árbol binario vacío (sin nodos aún).
- Una vista lista para imprimir.

---

## Ejecutar — el método principal

```csharp
public void Ejecutar()
{
    _vista.MostrarBanner();
    ConstruirArbol();
    RealizarBusquedas();
    RealizarActualizaciones();
    RealizarEliminaciones();
    MostrarRecorridos();
    MostrarMetricas();
    _vista.MostrarFin();
}
```

**¿Qué hace?** Es el único método público. `Program.cs` lo llama y desde aquí se ejecutan todos los casos de uso en el orden exacto que exige el enunciado. Cada línea es una llamada a otro método privado de esta misma clase.

---

## Caso 1 — ConstruirArbol

### El array de tuplas

```csharp
var nodos = new (string nombre, bool esCarpeta)[]
{
    ("Proyectos",  true ),
    ("Facturas",   true ),
    ...
};
```

**¿Qué es una tupla?** Un grupo de dos (o más) valores empaquetados juntos sin necesitar una clase. `(string nombre, bool esCarpeta)` es una tupla con dos campos nombrados.

**`var`** → el compilador detecta el tipo automáticamente. Es lo mismo que escribir `(string, bool)[]` pero más legible.

**¿Por qué este orden de inserción?** El orden de inserción determina la forma del árbol. `"Proyectos"` se inserta primero, así que se convierte en la raíz. Los siguientes se distribuyen a izquierda o derecha según su nombre.

### El foreach con desestructuración

```csharp
foreach (var (nombre, esCarpeta) in nodos)
{
    bool ok = _arbol.Insertar(nombre, esCarpeta, out int comp);
    _vista.MostrarInsercion(nombre, ok, comp);
}
```

**`var (nombre, esCarpeta)`** → **desestructuración de tupla**: extrae los dos valores de cada tupla directamente en variables separadas. Es más legible que `nodos[i].nombre`.

**`out int comp`** → declara la variable `comp` directamente al pasarla como parámetro `out`. Se puede hacer así desde C# 7.

**Patrón del Controller en acción:**
1. `_arbol.Insertar(...)` → pide al Model que haga el trabajo.
2. `_vista.MostrarInsercion(...)` → pasa el resultado a la View para mostrar.

### Prueba de duplicado

```csharp
bool dup = _arbol.Insertar("Proyectos", true, out int compDup);
_vista.MostrarInsercion("Proyectos", dup, compDup);
```

Intenta insertar `"Proyectos"` que ya existe. `dup` será `false` y la View mostrará el ✗ rojo. Esto demuestra que la política de duplicados funciona.

### Imprimir el árbol

```csharp
_vista.ImprimirArbol(_arbol.Raiz);
```

Pasa la raíz del árbol a la View. El Controller no sabe cómo se dibuja el árbol ASCII, solo entrega el dato.

---

## Caso 2 — RealizarBusquedas

```csharp
private void Buscar(string nombre)
{
    Nodo? resultado = _arbol.Buscar(nombre, out int comparaciones);
    _vista.MostrarResultadoBusqueda(nombre, resultado, comparaciones);
}
```

**¿Por qué hay un método auxiliar `Buscar`?** Para no repetir las dos líneas (pedir al Model + mostrar en View) seis veces. Se llama una vez por cada búsqueda.

**Las 6 búsquedas:**

```csharp
// 2 del subárbol izquierdo
Buscar("Contratos");
Buscar("Archivos");

// 2 del subárbol derecho
Buscar("Sistemas");
Buscar("Ventas");

// 2 inexistentes
Buscar("Presupuesto");
Buscar("Zonas");
```

Las dos inexistentes (`"Presupuesto"` y `"Zonas"`) están elegidas para que el algoritmo recorra varios niveles antes de confirmar que no existen, así el conteo de comparaciones es más informativo.

---

## Caso 3 — RealizarActualizaciones

```csharp
private void ActualizarNodo(string nombreAntiguo, string nombreNuevo)
{
    bool ok = _arbol.Actualizar(nombreAntiguo, nombreNuevo, out string mensaje);
    _vista.MostrarActualizacion(nombreAntiguo, nombreNuevo, ok, mensaje);
    _vista.ImprimirArbol(_arbol.Raiz);
}
```

**Misma estructura:** pedir al Model → mostrar en View → imprimir árbol.

**¿Por qué se imprime el árbol después de cada actualización?** Para que en consola se pueda ver cómo cambió el árbol con cada operación. El enunciado lo exige explícitamente para garantizar trazabilidad.

**Las 3 actualizaciones, en orden de dificultad:**

```csharp
// 1. Hoja: "Ventas" no tiene hijos → eliminación Caso 1
ActualizarNodo("Ventas", "VentasNacional");

// 2. Nodo con un solo hijo: "Sistemas" solo tiene "Tecnologia" → eliminación Caso 2
ActualizarNodo("Sistemas", "SistemasInfo");

// 3. Raíz: "Proyectos" tiene dos hijos → eliminación Caso 3 (sucesor)
ActualizarNodo("Proyectos", "ProyectosGlobales");
```

---

## Caso 4 — RealizarEliminaciones

```csharp
private void EliminarNodo(string nombre)
{
    bool ok = _arbol.Eliminar(nombre, out string caso);
    _vista.MostrarEliminacion(nombre, ok, caso);
    _vista.ImprimirArbol(_arbol.Raiz);
}
```

Misma estructura de siempre. El `caso` que retorna `ArbolBinario` describe cuál de los 3 casos BST se aplicó y se muestra en consola.

**Segunda eliminación — comentario importante:**
```csharp
// Contratos quedó con un solo hijo (Datos) tras eliminar Archivos
EliminarNodo("Contratos");
```

Este comentario explica por qué `"Contratos"` califica como Caso 2 aquí. Antes de eliminar `"Archivos"`, `"Contratos"` tenía dos hijos (`"Archivos"` y `"Datos"`). Después de eliminar `"Archivos"`, `"Contratos"` solo tiene `"Datos"`.

**Tercera eliminación — la raíz:**
```csharp
_vista.MostrarSubtitulo($"Eliminación 3 — Raíz  '{_arbol.Raiz?.Nombre}'  (dos hijos)");
EliminarNodo(_arbol.Raiz!.Nombre);
```

**`_arbol.Raiz?.Nombre`** → el **operador null-conditional** `?.`. Si `Raiz` es `null`, en vez de lanzar una excepción devuelve `null`. Protege contra errores si el árbol estuviera vacío.

**`_arbol.Raiz!.Nombre`** → el **null-forgiving operator** `!`. Le dice al compilador "en este punto sé con certeza que `Raiz` no es null". Sin él, el compilador advertiría un posible null.

**¿Por qué se obtiene el nombre dinámicamente?** Porque la raíz del árbol cambió durante las actualizaciones del Caso 3. Ya no es `"Proyectos"` sino otra cosa (el sucesor que quedó como raíz). Se usa `_arbol.Raiz.Nombre` para obtener lo que sea que sea la raíz en ese momento.

---

## Caso 5 — MostrarRecorridos

```csharp
_vista.MostrarRecorrido("Preorden",    _arbol.Preorden());
_vista.MostrarRecorrido("Inorden",     _arbol.Inorden());
_vista.MostrarRecorrido("Postorden",   _arbol.Postorden());
_vista.MostrarRecorrido("Por Niveles", _arbol.PorNiveles());
```

Cada método del Model retorna una `List<Nodo>` ya en el orden correcto. El Controller la pasa directamente a la View sin tocarla.

```csharp
_vista.MostrarMensaje("El recorrido INORDEN confirma que el BST sigue ordenado (lista alfabética).");
```

Justificación exigida por el enunciado. El inorden produce siempre una lista alfabética en un BST correcto. Si después de todas las operaciones el inorden sigue estando ordenado, significa que el árbol está bien.

---

## MostrarMetricas

```csharp
private void MostrarMetricas()
{
    _vista.MostrarEncabezado("MÉTRICAS FINALES");
    _vista.MostrarAltura(_arbol.Altura());
}
```

Una sola línea de trabajo: `_arbol.Altura()` calcula el número de niveles y `_vista.MostrarAltura()` lo imprime. El Controller solo conecta los dos.

---

## Resumen del patrón que se repite en todo el Controller

```
Paso 1: _arbol.Metodo(...)            → el Model hace el trabajo real
Paso 2: _vista.Mostrar...(resultado)  → la View muestra el resultado
Paso 3: _vista.ImprimirArbol(...)     → (cuando aplica) mostrar el árbol actualizado
```

El Controller **nunca** procesa datos, **nunca** imprime directamente, y **nunca** conoce la lógica interna del BST. Solo sabe cuándo llamar a qué método.
