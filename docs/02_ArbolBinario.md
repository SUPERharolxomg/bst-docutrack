# ArbolBinario.cs — Explicación detallada

## ¿Qué es este archivo?

Es el **cerebro del sistema**. Contiene toda la lógica que manipula el árbol: insertar nodos, buscarlos, eliminarlos, actualizarlos, recorrerlos y medir su altura. Nada de esto se muestra en pantalla desde aquí; solo se calculan y retornan resultados.

> Este archivo **no tiene ninguna línea de `Console`**. Todos los resultados los retorna para que el Controller los pase a la View.

---

## Propiedad principal

```csharp
public Nodo? Raiz { get; private set; }
```

**¿Qué es?** La puerta de entrada al árbol. Desde la raíz se puede llegar a cualquier otro nodo bajando por izquierda o derecha.

**¿Por qué `Nodo?` con el signo `?`?** Porque el árbol puede estar vacío. Cuando no se ha insertado ningún nodo, `Raiz` vale `null`.

**¿Por qué `private set`?** Porque nadie de afuera debe poder cambiar quién es la raíz directamente. Solo los métodos de esta clase lo hacen (al insertar o eliminar). Desde afuera solo se puede leer: `_arbol.Raiz`.

---

## INSERTAR

### Método público

```csharp
public bool Insertar(string nombre, bool esCarpeta, out int comparaciones)
```

**¿Qué recibe?**
- `nombre` → el nombre del nuevo nodo (ej: `"Facturas"`).
- `esCarpeta` → si es carpeta (`true`) o archivo (`false`).
- `out int comparaciones` → este es un **parámetro de salida**: el método lo llena con un número, y quien llamó al método puede leer ese número después. Se usa para contar cuántas comparaciones se hicieron antes de insertar.

**¿Qué retorna?**
- `true` → el nodo se insertó correctamente.
- `false` → el nombre ya existía (duplicado) o era vacío/nulo.

```csharp
comparaciones = 0;
if (string.IsNullOrWhiteSpace(nombre)) return false;
```

**Primera línea:** Todo parámetro `out` debe asignarse antes de que el método termine. Se pone en 0 al inicio porque aún no se ha hecho ninguna comparación.

**Segunda línea:** Valida que el nombre no sea `null`, cadena vacía `""` o solo espacios `"   "`. Si lo es, no tiene sentido insertar nada, se sale de inmediato con `false`.

```csharp
bool insertado = false;
Raiz = InsertarRec(Raiz, nombre.Trim(), esCarpeta, ref comparaciones, ref insertado);
return insertado;
```

**`bool insertado = false`** → bandera que el método recursivo cambiará a `true` si logra insertar.

**`nombre.Trim()`** → elimina espacios al inicio y al final del nombre para evitar duplicados por accidente (ej: `" Ventas"` y `"Ventas"` serían el mismo).

**`ref comparaciones` y `ref insertado`** → `ref` pasa las variables **por referencia**: el método recursivo modifica directamente estas variables, no copias. Sin `ref`, los cambios del recursivo se perderían al volver.

**`Raiz = InsertarRec(...)`** → el resultado del método recursivo se reasigna a `Raiz`. Esto es crítico para el primer nodo: cuando el árbol está vacío, `Raiz` es `null`, y el recursivo retorna el nodo nuevo, que se convierte en la nueva raíz.

---

### Método privado `InsertarRec` — la recursión real

```csharp
if (nodo == null)
{
    insertado = true;
    return new Nodo(nombre, esCarpeta);
}
```

**¿Cuándo llega un `null`?** Cuando el recorrido llegó a un espacio vacío donde no hay ningún nodo. Ese es exactamente el lugar donde hay que insertar.

**¿Qué hace aquí?**
1. Marca `insertado = true` → confirma que sí se insertó.
2. Crea y retorna el nuevo nodo. El padre lo recibirá y lo asignará a su hijo izquierdo o derecho.

```csharp
comparaciones++;
int cmp = string.Compare(nombre, nodo.Nombre, System.StringComparison.OrdinalIgnoreCase);
```

**`comparaciones++`** → cada vez que se llega a un nodo no nulo, se hizo una comparación. Se incrementa el contador.

**`string.Compare(..., OrdinalIgnoreCase)`** → compara dos strings ignorando si son mayúsculas o minúsculas. `"facturas"` y `"FACTURAS"` son iguales. Retorna:
- Número **negativo** → el primer string va antes alfabéticamente.
- **Cero** → los strings son iguales.
- Número **positivo** → el primer string va después alfabéticamente.

```csharp
if      (cmp < 0) nodo.Izquierdo = InsertarRec(nodo.Izquierdo, ...);
else if (cmp > 0) nodo.Derecho   = InsertarRec(nodo.Derecho, ...);
// cmp == 0: duplicado, insertado permanece false
```

**`cmp < 0`** → el nombre nuevo va antes → debe ir a la **izquierda** → llamar al recursivo con el hijo izquierdo.

**`cmp > 0`** → el nombre nuevo va después → debe ir a la **derecha** → llamar al recursivo con el hijo derecho.

**`cmp == 0`** → duplicado → no se hace nada. `insertado` queda en `false`. El árbol no se modifica.

**¿Por qué se reasigna `nodo.Izquierdo`?** Porque el recursivo puede retornar un nodo nuevo (cuando llega a un `null`). Si no se reasignara, el hijo nuevo se crearía pero nadie lo guardaría y se perdería.

```csharp
return nodo;
```

Retorna el nodo actual sin modificarlo. Esto "sube" por la pila de recursión para que cada padre reasigne correctamente a su hijo.

---

## BUSCAR

```csharp
public Nodo? Buscar(string nombre, out int comparaciones)
```

**¿Qué hace?** Recorre el árbol buscando el nodo con ese nombre. Retorna el nodo si lo encuentra, o `null` si no existe.

El recorrido es idéntico al de insertar: en cada nodo compara el nombre y baja por izquierda o derecha. La diferencia es que cuando llega a `null`, en vez de crear un nodo nuevo, simplemente retorna `null` (no existe).

**Ventaja del BST:** En cada comparación se descarta **la mitad del árbol**. No hay que revisar todos los nodos, solo el camino directo.

---

## ELIMINAR

```csharp
public bool Eliminar(string nombre, out string casoEliminacion)
```

**`out string casoEliminacion`** → texto que describe cuál de los 3 casos del BST se aplicó. Lo muestra la View para que sea trazable en consola.

**¿Por qué hay 3 casos?** Porque eliminar un nodo del BST es diferente según cuántos hijos tenga. No se puede simplemente "borrar" el nodo porque habría que reconectar sus hijos al árbol sin romper el orden.

---

### Caso 1 — Nodo hoja (sin hijos)

```csharp
if (nodo.Izquierdo == null && nodo.Derecho == null)
{
    caso = "Caso 1 – Nodo hoja: desenlazado directamente.";
    return null;
}
```

**¿Qué pasa?** El nodo no tiene hijos. Retornar `null` hace que el padre deje de apuntar a él. El nodo queda desconectado del árbol y el recolector de basura de .NET lo elimina de la memoria.

**Ejemplo:**
```
Antes:  Contratos → [Archivos]   Después:  Contratos → null
```

---

### Caso 2 — Un solo hijo

```csharp
if (nodo.Izquierdo == null)
{
    caso = "Caso 2 – Un hijo (derecho): hijo reconectado con el padre.";
    return nodo.Derecho;
}
if (nodo.Derecho == null)
{
    caso = "Caso 2 – Un hijo (izquierdo): hijo reconectado con el padre.";
    return nodo.Izquierdo;
}
```

**¿Qué pasa?** El nodo tiene un solo hijo. Al retornar ese hijo, el padre del nodo eliminado apuntará directamente al nieto, "saltando" al nodo eliminado.

**Ejemplo:**
```
Antes:  Padre → [Sistemas] → [Tecnologia]
Después: Padre → [Tecnologia]
```
`Sistemas` queda desconectado. `Tecnologia` sube un nivel.

---

### Caso 3 — Dos hijos (el más complejo)

```csharp
Nodo sucesor = ObtenerMinimo(nodo.Derecho);
```

**¿Qué es el sucesor?** El nodo con el nombre más pequeño del subárbol derecho. En orden alfabético, es el que viene **inmediatamente después** del nodo a eliminar.

**¿Por qué el sucesor?** No se puede simplemente eliminar el nodo con dos hijos porque habría que elegir cuál de los dos hijos sube, y cualquier elección podría romper el BST. El sucesor tiene la propiedad de que es mayor que todos los nodos del subárbol izquierdo y menor que todos los del subárbol derecho, así el orden se mantiene.

```csharp
nodo.Nombre    = sucesor.Nombre;
nodo.EsCarpeta = sucesor.EsCarpeta;
```

**¿Qué pasa aquí?** No se borra el nodo físicamente. En cambio, se **copian los datos del sucesor** al nodo actual. El nodo actual "se convierte" en el sucesor sin moverse de su posición en el árbol.

```csharp
bool dummy = false; string casoInterno = string.Empty;
nodo.Derecho = EliminarRec(nodo.Derecho, sucesor.Nombre, ref dummy, ref casoInterno);
```

Ahora hay dos nodos con el mismo nombre. Se elimina el sucesor **original** del subárbol derecho. El sucesor siempre tiene 0 o 1 hijo (nunca 2), así que su eliminación cae en Caso 1 o Caso 2. `dummy` y `casoInterno` son variables desechables porque esa eliminación interna no se reporta.

---

### `ObtenerMinimo`

```csharp
while (nodo.Izquierdo != null) nodo = nodo.Izquierdo;
return nodo;
```

**¿Cómo funciona?** En un BST, el nodo más pequeño **siempre está al final del camino más a la izquierda**. Este método baja por la izquierda hasta que no hay más izquierda (`Izquierdo == null`), y ese es el mínimo.

---

## ACTUALIZAR

**¿Por qué no se edita el nodo directamente?** Porque cambiar el nombre de un nodo en su lugar rompería el BST. El nodo quedaría en la posición del nombre viejo, pero con el nombre nuevo, y ya no estaría en el lugar correcto. Por eso la estrategia obligatoria es:

1. **Eliminar** el nodo con el nombre viejo (se reconectan sus hijos al árbol).
2. **Insertar** un nodo nuevo con el nombre nuevo (se coloca en la posición correcta).

```csharp
bool esCarpeta = objetivo.EsCarpeta;  // guardar ANTES de eliminar
```

**¿Por qué guardar `EsCarpeta` antes?** En el Caso 3 de eliminación, el nodo no desaparece físicamente: sus datos son sobreescritos por el sucesor. Si se leyera `EsCarpeta` después del `Eliminar`, se obtendría el tipo del sucesor, no el del nodo original que se quería actualizar.

```csharp
Eliminar(nombreAntiguo, out string casoElim);
Insertar(nombreNuevo, esCarpeta, out int _);
```

`out int _` → el guion bajo `_` es el **discard**: descarta el valor de comparaciones de la inserción porque no se necesita reportar.

---

## RECORRIDOS

Todos los recorridos recursivos comparten la misma estructura: visitar nodo izquierdo, nodo actual y nodo derecho, cambiando solo el **orden** en que se agrega el nodo a la lista.

| Recorrido | Orden | Cuándo usar |
|-----------|-------|-------------|
| **Preorden** | Raíz → Izq → Der | Para clonar o serializar el árbol |
| **Inorden** | Izq → Raíz → Der | Para **verificar que el BST está ordenado** (produce lista alfabética) |
| **Postorden** | Izq → Der → Raíz | Para liberar memoria (hijos antes que padre) |
| **Por Niveles** | Nivel a nivel | Para ver la estructura por capas |

### PorNiveles con Queue

```csharp
var cola = new Queue<Nodo>();
cola.Enqueue(Raiz);
```

**¿Por qué una `Queue`?** Porque es FIFO (First In, First Out): el primero en entrar es el primero en salir. Esto garantiza que se procesen todos los nodos de un nivel antes de pasar al siguiente.

```csharp
Nodo actual = cola.Dequeue();   // saca el nodo del frente
lista.Add(actual);
cola.Enqueue(actual.Izquierdo); // mete sus hijos al FINAL
cola.Enqueue(actual.Derecho);
```

Los hijos se meten al **final** de la cola. Como ya hay nodos del nivel actual al frente, esos se procesan primero. Los hijos (nivel siguiente) esperan su turno al final.

---

## ALTURA

```csharp
if (n == null) return 0;
return 1 + System.Math.Max(AlturaRec(n.Izquierdo), AlturaRec(n.Derecho));
```

**¿Cómo funciona?**
- Si el nodo es `null` → no existe → altura **0** (caso base, detiene la recursión).
- Si el nodo existe → cuenta **1** (este nivel) más la altura del subárbol más profundo.
- `Math.Max` elige el mayor entre el subárbol izquierdo y el derecho.
- La recursión baja hasta todas las hojas y "sube" acumulando 1 por nivel.
