namespace DocuTrack.Model
{
    public class Nodo
    {
        public string Nombre    { get; set; }  // clave del BST
        public bool   EsCarpeta { get; set; }  // true = carpeta, false = archivo
        public Nodo?  Izquierdo { get; set; }
        public Nodo?  Derecho   { get; set; }

        public Nodo(string nombre, bool esCarpeta)
        {
            Nombre    = nombre;
            EsCarpeta = esCarpeta;
            Izquierdo = null;
            Derecho   = null;
        }

        public string Icono => EsCarpeta ? "[DIR] " : "[FILE]";
    }
}
