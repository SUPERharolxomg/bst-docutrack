using System.Collections.Generic;

namespace DocuTrack.Model
{
    // BST con clave Nombre (OrdinalIgnoreCase). Sin referencias a Console.
    public class ArbolBinario
    {
        public Nodo? Raiz { get; private set; }

        // Retorna true si se insertó; false si el nombre es duplicado o vacío.
        public bool Insertar(string nombre, bool esCarpeta, out int comparaciones)
        {
            comparaciones = 0;
            if (string.IsNullOrWhiteSpace(nombre)) return false;

            bool insertado = false;
            Raiz = InsertarRec(Raiz, nombre.Trim(), esCarpeta, ref comparaciones, ref insertado);
            return insertado;
        }

        private Nodo InsertarRec(Nodo? nodo, string nombre, bool esCarpeta,
                                  ref int comparaciones, ref bool insertado)
        {
            if (nodo == null)
            {
                insertado = true;
                return new Nodo(nombre, esCarpeta);
            }

            comparaciones++;
            int cmp = string.Compare(nombre, nodo.Nombre, System.StringComparison.OrdinalIgnoreCase);

            if      (cmp < 0) nodo.Izquierdo = InsertarRec(nodo.Izquierdo, nombre, esCarpeta, ref comparaciones, ref insertado);
            else if (cmp > 0) nodo.Derecho   = InsertarRec(nodo.Derecho,   nombre, esCarpeta, ref comparaciones, ref insertado);
            // cmp == 0: duplicado, insertado permanece false

            return nodo;
        }

        // Retorna el nodo encontrado o null si no existe.
        public Nodo? Buscar(string nombre, out int comparaciones)
        {
            comparaciones = 0;
            if (string.IsNullOrWhiteSpace(nombre)) return null;
            return BuscarRec(Raiz, nombre.Trim(), ref comparaciones);
        }

        private Nodo? BuscarRec(Nodo? nodo, string nombre, ref int comparaciones)
        {
            if (nodo == null) return null;

            comparaciones++;
            int cmp = string.Compare(nombre, nodo.Nombre, System.StringComparison.OrdinalIgnoreCase);

            if (cmp == 0) return nodo;
            if (cmp < 0)  return BuscarRec(nodo.Izquierdo, nombre, ref comparaciones);
            return BuscarRec(nodo.Derecho, nombre, ref comparaciones);
        }

        // Elimina el nodo e informa en 'casoEliminacion' cuál de los 3 casos BST se aplicó.
        public bool Eliminar(string nombre, out string casoEliminacion)
        {
            casoEliminacion = string.Empty;
            if (string.IsNullOrWhiteSpace(nombre)) return false;

            bool eliminado = false;
            Raiz = EliminarRec(Raiz, nombre.Trim(), ref eliminado, ref casoEliminacion);
            return eliminado;
        }

        private Nodo? EliminarRec(Nodo? nodo, string nombre, ref bool eliminado, ref string caso)
        {
            if (nodo == null) return null;

            int cmp = string.Compare(nombre, nodo.Nombre, System.StringComparison.OrdinalIgnoreCase);

            if      (cmp < 0) nodo.Izquierdo = EliminarRec(nodo.Izquierdo, nombre, ref eliminado, ref caso);
            else if (cmp > 0) nodo.Derecho   = EliminarRec(nodo.Derecho,   nombre, ref eliminado, ref caso);
            else
            {
                eliminado = true;

                // Caso 1: hoja
                if (nodo.Izquierdo == null && nodo.Derecho == null)
                {
                    caso = "Caso 1 – Nodo hoja: desenlazado directamente.";
                    return null;
                }
                // Caso 2: un solo hijo
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
                // Caso 3: dos hijos → se reemplaza por el sucesor (mínimo del subárbol derecho)
                Nodo sucesor = ObtenerMinimo(nodo.Derecho);
                caso = $"Caso 3 – Dos hijos: reemplazado por sucesor '{sucesor.Nombre}'.";
                nodo.Nombre    = sucesor.Nombre;
                nodo.EsCarpeta = sucesor.EsCarpeta;

                bool dummy = false; string casoInterno = string.Empty;
                nodo.Derecho = EliminarRec(nodo.Derecho, sucesor.Nombre, ref dummy, ref casoInterno);
            }

            return nodo;
        }

        // Devuelve el nodo con el nombre más pequeño del subárbol dado.
        private Nodo ObtenerMinimo(Nodo nodo)
        {
            while (nodo.Izquierdo != null) nodo = nodo.Izquierdo;
            return nodo;
        }

        // Estrategia: Eliminar(viejo) + Insertar(nuevo). Preserva el tipo (EsCarpeta).
        public bool Actualizar(string nombreAntiguo, string nombreNuevo, out string mensaje)
        {
            mensaje = string.Empty;

            if (string.IsNullOrWhiteSpace(nombreAntiguo) || string.IsNullOrWhiteSpace(nombreNuevo))
            {
                mensaje = "Error: nombre inválido (nulo o vacío).";
                return false;
            }

            int dummy;
            if (Buscar(nombreNuevo, out dummy) != null)
            {
                mensaje = $"El nombre '{nombreNuevo}' ya existe. Actualización cancelada.";
                return false;
            }

            Nodo? objetivo = Buscar(nombreAntiguo, out dummy);
            if (objetivo == null)
            {
                mensaje = $"No se encontró '{nombreAntiguo}'. Actualización cancelada.";
                return false;
            }

            bool esCarpeta = objetivo.EsCarpeta; // guardar antes de eliminar (Caso 3 sobreescribe el nodo)

            Eliminar(nombreAntiguo, out string casoElim);
            Insertar(nombreNuevo, esCarpeta, out int _);

            mensaje = $"Nodo '{nombreAntiguo}' → '{nombreNuevo}' | {casoElim}";
            return true;
        }

        // Preorden: Raíz → Izq → Der
        public List<Nodo> Preorden()
        {
            var lista = new List<Nodo>();
            PreordenRec(Raiz, lista);
            return lista;
        }
        private void PreordenRec(Nodo? n, List<Nodo> lista)
        {
            if (n == null) return;
            lista.Add(n);
            PreordenRec(n.Izquierdo, lista);
            PreordenRec(n.Derecho,   lista);
        }

        // Inorden: Izq → Raíz → Der (produce lista ordenada; verifica la propiedad del BST)
        public List<Nodo> Inorden()
        {
            var lista = new List<Nodo>();
            InordenRec(Raiz, lista);
            return lista;
        }
        private void InordenRec(Nodo? n, List<Nodo> lista)
        {
            if (n == null) return;
            InordenRec(n.Izquierdo, lista);
            lista.Add(n);
            InordenRec(n.Derecho,   lista);
        }

        // Postorden: Izq → Der → Raíz
        public List<Nodo> Postorden()
        {
            var lista = new List<Nodo>();
            PostordenRec(Raiz, lista);
            return lista;
        }
        private void PostordenRec(Nodo? n, List<Nodo> lista)
        {
            if (n == null) return;
            PostordenRec(n.Izquierdo, lista);
            PostordenRec(n.Derecho,   lista);
            lista.Add(n);
        }

        // Por niveles con Queue (FIFO): procesa un nivel completo antes de bajar al siguiente.
        public List<Nodo> PorNiveles()
        {
            var lista = new List<Nodo>();
            if (Raiz == null) return lista;

            var cola = new Queue<Nodo>();
            cola.Enqueue(Raiz);

            while (cola.Count > 0)
            {
                Nodo actual = cola.Dequeue();
                lista.Add(actual);
                if (actual.Izquierdo != null) cola.Enqueue(actual.Izquierdo);
                if (actual.Derecho   != null) cola.Enqueue(actual.Derecho);
            }
            return lista;
        }

        // Altura = 1 + max(altura izq, altura der). Caso base: nodo null → 0.
        public int Altura() => AlturaRec(Raiz);

        private int AlturaRec(Nodo? n)
        {
            if (n == null) return 0;
            return 1 + System.Math.Max(AlturaRec(n.Izquierdo), AlturaRec(n.Derecho));
        }
    }
}
