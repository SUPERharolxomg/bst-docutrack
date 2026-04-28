using DocuTrack.Controller;

// Punto de entrada de la aplicación.
// Solo configura la consola e instancia el Controller; ninguna lógica aquí.

// UTF-8 necesario para mostrar correctamente los caracteres del árbol ASCII: ═ │ ├ └ ╔ ║
System.Console.OutputEncoding = System.Text.Encoding.UTF8;

var controller = new ArbolController();
controller.Iniciar();
