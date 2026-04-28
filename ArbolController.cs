using DocuTrack.Model;
using DocuTrack.View;

namespace DocuTrack.Controller
{
    // Orquesta los casos de uso en orden. No contiene lógica del BST ni usa Console.
    public class ArbolController
    {
        private readonly ArbolBinario _arbol;
        private readonly ArbolView    _vista;

        public ArbolController()
        {
            _arbol = new ArbolBinario();
            _vista = new ArbolView();
        }

        public void Iniciar()
        {
            _vista.MostrarBanner();
            int opcion;
            do
            {
                _vista.MostrarMenu();
                opcion = _vista.LeerOpcion();
                switch (opcion)
                {
                    case 1: InsertarInteractivo();   break;
                    case 2: ListarInteractivo();     break;
                    case 3: BuscarInteractivo();     break;
                    case 4: ActualizarInteractivo(); break;
                    case 5: EliminarInteractivo();   break;
                    case 6: _vista.MostrarMensaje("Saliendo..."); break;
                    default: _vista.MostrarMensaje("Opción inválida."); break;
                }
            } while (opcion != 6);
            _vista.MostrarFin();
        }

        private void InsertarInteractivo()
        {
            string nombre    = _vista.LeerTexto("Nombre a insertar: ");
            bool   esCarpeta = _vista.LeerEsCarpeta();
            bool   ok        = _arbol.Insertar(nombre, esCarpeta, out int comp);
            _vista.MostrarInsercion(nombre, ok, comp);
            _vista.ImprimirArbol(_arbol.Raiz);
        }

        private void ListarInteractivo()
        {
            _vista.MostrarListaSimple(_arbol.Inorden());
        }

        private void BuscarInteractivo()
        {
            string nombre    = _vista.LeerTexto("Nombre a buscar: ");
            Nodo?  resultado = _arbol.Buscar(nombre, out int comp);
            _vista.MostrarResultadoBusqueda(nombre, resultado, comp);
        }

        private void ActualizarInteractivo()
        {
            string antiguo = _vista.LeerTexto("Nombre actual:  ");
            string nuevo   = _vista.LeerTexto("Nuevo nombre:   ");
            bool   ok      = _arbol.Actualizar(antiguo, nuevo, out string mensaje);
            _vista.MostrarActualizacion(antiguo, nuevo, ok, mensaje);
            if (ok) _vista.ImprimirArbol(_arbol.Raiz);
        }

        private void EliminarInteractivo()
        {
            string nombre = _vista.LeerTexto("Nombre a eliminar: ");
            bool   ok     = _arbol.Eliminar(nombre, out string caso);
            _vista.MostrarEliminacion(nombre, ok, caso);
            if (ok) _vista.ImprimirArbol(_arbol.Raiz);
        }

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

        private void ConstruirArbol()
        {
            _vista.MostrarEncabezado("CASO 1 — CONSTRUCCIÓN DEL ÁRBOL  (14 nodos)");
            _vista.MostrarMensaje("Insertando mezcla de carpetas y archivos:");
            _vista.MostrarMensaje("");

            var nodos = new (string nombre, bool esCarpeta)[]
            {
                ("Proyectos",  true ),   // raíz
                ("Facturas",   true ),   // subárbol izquierdo
                ("Reportes",   true ),   // subárbol derecho
                ("Contratos",  true ),
                ("Imagenes",   false),
                ("Archivos",   false),   // hoja izquierda
                ("Datos",      false),   // hoja izquierda
                ("Nominas",    false),   // hoja derecha
                ("Sistemas",   true ),   // nodo con un solo hijo
                ("Historicos", false),
                ("Legal",      false),
                ("Tecnologia", true ),   // nodo con un solo hijo
                ("Usuarios",   true ),   // nodo con un solo hijo
                ("Ventas",     false),   // hoja derecha
            };

            foreach (var (nombre, esCarpeta) in nodos)
            {
                bool ok = _arbol.Insertar(nombre, esCarpeta, out int comp);
                _vista.MostrarInsercion(nombre, ok, comp);
            }

            // Prueba de la política de duplicados
            _vista.MostrarMensaje("");
            _vista.MostrarMensaje("── Prueba de duplicado (debe rechazarse) ──");
            bool dup = _arbol.Insertar("Proyectos", true, out int compDup);
            _vista.MostrarInsercion("Proyectos", dup, compDup);

            _vista.ImprimirArbol(_arbol.Raiz);
        }

        private void RealizarBusquedas()
        {
            _vista.MostrarEncabezado("CASO 2 — BÚSQUEDAS RÁPIDAS  (6 búsquedas)");

            _vista.MostrarSubtitulo("2 existentes en el subárbol IZQUIERDO");
            Buscar("Contratos");
            Buscar("Archivos");

            _vista.MostrarSubtitulo("2 existentes en el subárbol DERECHO");
            Buscar("Sistemas");
            Buscar("Ventas");

            _vista.MostrarSubtitulo("2 inexistentes");
            Buscar("Presupuesto");
            Buscar("Zonas");
        }

        private void Buscar(string nombre)
        {
            Nodo? resultado = _arbol.Buscar(nombre, out int comparaciones);
            _vista.MostrarResultadoBusqueda(nombre, resultado, comparaciones);
        }

        private void RealizarActualizaciones()
        {
            _vista.MostrarEncabezado("CASO 3 — ACTUALIZACIONES SELECTIVAS");

            _vista.MostrarSubtitulo("Actualización 1 — Hoja  'Ventas' → 'VentasNacional'");
            ActualizarNodo("Ventas", "VentasNacional");

            _vista.MostrarSubtitulo("Actualización 2 — Hijo con un hijo  'Sistemas' → 'SistemasInfo'");
            ActualizarNodo("Sistemas", "SistemasInfo");

            _vista.MostrarSubtitulo("Actualización 3 — Raíz  'Proyectos' → 'ProyectosGlobales'");
            ActualizarNodo("Proyectos", "ProyectosGlobales");
        }

        private void ActualizarNodo(string nombreAntiguo, string nombreNuevo)
        {
            bool ok = _arbol.Actualizar(nombreAntiguo, nombreNuevo, out string mensaje);
            _vista.MostrarActualizacion(nombreAntiguo, nombreNuevo, ok, mensaje);
            _vista.ImprimirArbol(_arbol.Raiz);
        }

        private void RealizarEliminaciones()
        {
            _vista.MostrarEncabezado("CASO 4 — ELIMINACIONES SELECTIVAS");

            _vista.MostrarSubtitulo("Eliminación 1 — Hoja  'Archivos'");
            EliminarNodo("Archivos");

            // Contratos quedó con un solo hijo (Datos) tras eliminar Archivos
            _vista.MostrarSubtitulo("Eliminación 2 — Nodo con un hijo  'Contratos'");
            EliminarNodo("Contratos");

            // La raíz actual se obtiene dinámicamente porque cambió tras las actualizaciones
            _vista.MostrarSubtitulo($"Eliminación 3 — Raíz  '{_arbol.Raiz?.Nombre}'  (dos hijos)");
            EliminarNodo(_arbol.Raiz!.Nombre);
        }

        private void EliminarNodo(string nombre)
        {
            bool ok = _arbol.Eliminar(nombre, out string caso);
            _vista.MostrarEliminacion(nombre, ok, caso);
            _vista.ImprimirArbol(_arbol.Raiz);
        }

        private void MostrarRecorridos()
        {
            _vista.MostrarEncabezado("CASO 5 — RECORRIDOS DE VERIFICACIÓN");
            _vista.MostrarMensaje("");
            _vista.MostrarRecorrido("Preorden",    _arbol.Preorden());
            _vista.MostrarRecorrido("Inorden",     _arbol.Inorden());
            _vista.MostrarRecorrido("Postorden",   _arbol.Postorden());
            _vista.MostrarRecorrido("Por Niveles", _arbol.PorNiveles());
            _vista.MostrarMensaje("");
            _vista.MostrarMensaje("El recorrido INORDEN confirma que el BST sigue ordenado (lista alfabética).");
        }

        private void MostrarMetricas()
        {
            _vista.MostrarEncabezado("MÉTRICAS FINALES");
            _vista.MostrarAltura(_arbol.Altura());
        }
    }
}
