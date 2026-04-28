using System;
using System.Collections.Generic;
using DocuTrack.Model;

namespace DocuTrack.View
{
    // Única clase autorizada a usar Console. Solo presenta datos; no decide nada.
    public class ArbolView
    {
        public void MostrarBanner()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║        DocuTrack S.A. — Gestión Jerárquica de Documentos    ║");
            Console.WriteLine("  ║            Árbol Binario de Búsqueda  (BST)  — MVC C#       ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }

        public void MostrarFin()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("  ════════════════════  FIN DE LA EJECUCIÓN  ════════════════════");
            Console.ResetColor();
            Console.WriteLine();
        }

        public void MostrarEncabezado(string titulo)
        {
            Console.WriteLine();
            Console.WriteLine(new string('═', 65));
            Console.WriteLine($"  ▶  {titulo}");
            Console.WriteLine(new string('═', 65));
        }

        public void MostrarSubtitulo(string texto)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  ┌─ {texto}");
            Console.ResetColor();
        }

        // Imprime el árbol con conectores ├──, └──, │.
        // 'esUltimo' decide el conector y si la rama vertical continúa o no.
        public void ImprimirArbol(Nodo? raiz)
        {
            Console.WriteLine();
            Console.WriteLine("  ┌──────────────────────────────────────────────────────┐");
            Console.WriteLine("  │            ÁRBOL BINARIO  —  DocuTrack S.A.          │");
            Console.WriteLine("  └──────────────────────────────────────────────────────┘");

            if (raiz == null) { Console.WriteLine("  (árbol vacío)"); Console.WriteLine(); return; }

            ImprimirNodoRec(raiz, "  ", esUltimo: true);
            Console.WriteLine();
        }

        private void ImprimirNodoRec(Nodo nodo, string prefijo, bool esUltimo)
        {
            string conector    = esUltimo ? "└── " : "├── ";
            string continuador = esUltimo ? "    " : "│   ";

            Console.ForegroundColor = nodo.EsCarpeta ? ConsoleColor.Yellow : ConsoleColor.White;
            Console.WriteLine(prefijo + conector + nodo.Icono + " " + nodo.Nombre);
            Console.ResetColor();

            string nuevoPrefijo = prefijo + continuador;
            bool tieneIzq = nodo.Izquierdo != null;
            bool tieneDer = nodo.Derecho   != null;

            if (tieneIzq && tieneDer)
            {
                ImprimirNodoRec(nodo.Izquierdo!, nuevoPrefijo, esUltimo: false);
                ImprimirNodoRec(nodo.Derecho!,   nuevoPrefijo, esUltimo: true);
            }
            else if (tieneIzq) ImprimirNodoRec(nodo.Izquierdo!, nuevoPrefijo, esUltimo: true);
            else if (tieneDer) ImprimirNodoRec(nodo.Derecho!,   nuevoPrefijo, esUltimo: true);
        }

        public void MostrarInsercion(string nombre, bool insertado, int comparaciones)
        {
            if (insertado)
                Console.WriteLine($"    ✓  Insertado : '{nombre}'  |  comparaciones: {comparaciones}");
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"    ✗  Duplicado rechazado: '{nombre}' ya existe en el árbol.");
                Console.ResetColor();
            }
        }

        public void MostrarResultadoBusqueda(string nombre, Nodo? nodo, int comparaciones)
        {
            Console.Write($"    Buscar('{nombre}')  →  ");
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
        }

        public void MostrarActualizacion(string nombreAntiguo, string nombreNuevo, bool exito, string mensaje)
        {
            if (exito)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"    ✓  Actualización: '{nombreAntiguo}' → '{nombreNuevo}'");
                Console.ResetColor();
                Console.WriteLine($"       Detalle: {mensaje}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"    ✗  Actualización fallida: {mensaje}");
                Console.ResetColor();
            }
        }

        public void MostrarEliminacion(string nombre, bool eliminado, string caso)
        {
            if (eliminado)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"    ✓  Eliminado: '{nombre}'");
                Console.ResetColor();
                Console.WriteLine($"       {caso}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"    ✗  No encontrado para eliminar: '{nombre}'");
                Console.ResetColor();
            }
        }

        // {tipo,-12} alinea el nombre del recorrido a 12 chars para que queden en columna.
        public void MostrarRecorrido(string tipo, List<Nodo> nodos)
        {
            Console.Write($"    {tipo,-12}: ");
            foreach (Nodo n in nodos)
                Console.Write($"{n.Nombre}({(n.EsCarpeta ? "D" : "F")})  ");
            Console.WriteLine();
        }

        public void MostrarAltura(int altura)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"    Altura del árbol final: {altura} nivel(es)");
            Console.ResetColor();
        }

        public void MostrarMensaje(string mensaje) => Console.WriteLine($"    {mensaje}");

        public void MostrarMenu()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ╔══════════════════════════════════════╗");
            Console.WriteLine("  ║    DocuTrack — Menú Principal        ║");
            Console.WriteLine("  ╠══════════════════════════════════════╣");
            Console.WriteLine("  ║  1. Insertar documento / carpeta     ║");
            Console.WriteLine("  ║  2. Listar todos (inorden)           ║");
            Console.WriteLine("  ║  3. Buscar                           ║");
            Console.WriteLine("  ║  4. Actualizar nombre                ║");
            Console.WriteLine("  ║  5. Eliminar                         ║");
            Console.WriteLine("  ║  6. Salir                            ║");
            Console.WriteLine("  ╚══════════════════════════════════════╝");
            Console.ResetColor();
            Console.Write("  Seleccione una opción: ");
        }

        public int LeerOpcion()
        {
            int opcion;
            while (!int.TryParse(Console.ReadLine(), out opcion))
                Console.Write("  Entrada inválida. Intente de nuevo: ");
            return opcion;
        }

        public string LeerTexto(string prompt)
        {
            string input;
            Console.Write($"  {prompt}");
            do
            {
                input = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(input))
                    Console.Write("  Entrada inválida. Intente de nuevo: ");
            } while (string.IsNullOrWhiteSpace(input));
            return input.Trim();
        }

        public bool LeerEsCarpeta()
        {
            Console.Write("  ¿Es carpeta? (s/n): ");
            string resp = Console.ReadLine() ?? "n";
            return resp.Trim().ToLower() == "s";
        }

        public void MostrarListaSimple(List<Nodo> nodos)
        {
            Console.WriteLine();
            if (nodos.Count == 0)
            {
                Console.WriteLine("    (árbol vacío)");
                return;
            }
            Console.WriteLine("    Elementos en el árbol (inorden):");
            foreach (Nodo n in nodos)
                Console.WriteLine($"      {n.Icono} {n.Nombre}");
        }
    }
}
