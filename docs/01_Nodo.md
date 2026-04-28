# 📄 Nodo.cs — Explicación detallada

## ¿Qué es este archivo?

Este archivo define **la pieza más pequeña del sistema**: el nodo. Un nodo representa una sola carpeta o un solo archivo dentro del árbol de DocuTrack. Por sí solo un nodo no hace nada; es solo un contenedor de datos. El árbol se construye conectando nodos entre sí.

> ⚠️ Este archivo **no tiene ninguna línea de Console**. Solo define datos. Esa es la regla del Model en MVC.

---

## El código, explicado parte por parte

### 1. El espacio de nombres

```csharp
namespace DocuTrack.Model
```

Esto agrupa la clase dentro de la capa **Model** del patrón MVC. Cuando `ArbolBinario` o `ArbolController` quieren usar `Nodo`, deben importar este espacio de nombres con `using DocuTrack.Model`.

---

### 2. Declaración de la clase

```csharp
public class Nodo
```

`public` significa que cualquier otro archivo del proyecto puede usar esta clase. Sin `public`, solo podría usarse dentro del mismo archivo.

---

### 3. La clave del árbol — `Nombre`

```csharp
public string Nombre { get; set; }  // clave del BST
```

**¿Qué guarda?** El nombre del nodo: `"Proyectos"`, `"Facturas"`, `"Ventas"`, etc.

**¿Para qué se usa?** Es la **clave de ordenamiento** del árbol binario. Cada vez que se inserta un nodo nuevo, se compara su `Nombre` con el nodo actual para decidir:
- Si es **menor** → va a la izquierda.
- Si es **mayor** → va a la derecha.
- Si es **igual** → es duplicado, se rechaza.

**¿Qué significa `{ get; set; }`?**
- `get` → permite leer el valor desde fuera: `nodo.Nombre`.
- `set` → permite modificarlo desde fuera: `nodo.Nombre = "nuevo"`.

---

### 4. El tipo del nodo — `EsCarpeta`

```csharp
public bool EsCarpeta { get; set; }  // true = carpeta, false = archivo
```

**¿Qué guarda?** Un valor verdadero o falso que indica qué tipo de nodo es:
- `true` → es una **carpeta** (puede tener 0, 1 o 2 hijos).
- `false` → es un **archivo** (siempre es hoja, sin hijos).

**¿Quién lo usa?** La View lo usa para elegir el color y el ícono al imprimir el árbol:
- Carpetas → color amarillo, ícono `[DIR]`.
- Archivos → color blanco, ícono `[FILE]`.

---

### 5. Los hijos — `Izquierdo` y `Derecho`

```csharp
public Nodo? Izquierdo { get; set; }
public Nodo? Derecho   { get; set; }
```

**¿Qué guardan?** Cada nodo tiene dos "flechas" que apuntan a sus hijos:
- `Izquierdo` → apunta al nodo cuyo nombre viene **antes** alfabéticamente.
- `Derecho` → apunta al nodo cuyo nombre viene **después** alfabéticamente.

**¿Qué significa el `?`?** Indica que estas propiedades pueden ser `null`. Cuando un nodo no tiene hijo izquierdo o derecho, esa propiedad vale `null`. Si un nodo tiene ambos hijos en `null`, se llama **hoja**.

**¿Quién los asigna?** `ArbolBinario` los conecta y desconecta al insertar o eliminar nodos.

---

### 6. El constructor

```csharp
public Nodo(string nombre, bool esCarpeta)
{
    Nombre    = nombre;
    EsCarpeta = esCarpeta;
    Izquierdo = null;
    Derecho   = null;
}
```

**¿Qué es un constructor?** Es el método que se ejecuta automáticamente cuando se escribe `new Nodo(...)`. Es la "fábrica" que crea el objeto.

**¿Qué hace aquí?**
1. Asigna el `nombre` recibido a la propiedad `Nombre`.
2. Asigna el `esCarpeta` recibido a la propiedad `EsCarpeta`.
3. Pone `Izquierdo = null` → todo nodo **nace sin hijo izquierdo**.
4. Pone `Derecho = null` → todo nodo **nace sin hijo derecho**.

Cuando nace, todo nodo es una hoja. Los hijos se conectan después cuando se insertan nuevos nodos en el árbol.

---

### 7. El ícono visual — `Icono`

```csharp
public string Icono => EsCarpeta ? "[DIR] " : "[FILE]";
```

**¿Qué hace?** Devuelve el texto visual que la View usa al imprimir cada línea del árbol ASCII.

**¿Cómo se lee?**
- Si `EsCarpeta` es `true` → devuelve `"[DIR] "`.
- Si `EsCarpeta` es `false` → devuelve `"[FILE]"`.

**¿Qué significa `=>`?** Es una **expresión de cuerpo** (forma corta). Es exactamente lo mismo que escribir:
```csharp
public string Icono
{
    get { return EsCarpeta ? "[DIR] " : "[FILE]"; }
}
```

**¿Quién la usa?** `ArbolView.ImprimirNodoRec` la usa al construir cada línea del árbol:
```
└── [DIR]  Proyectos
    ├── [DIR]  Facturas
    └── [FILE] Ventas
```
