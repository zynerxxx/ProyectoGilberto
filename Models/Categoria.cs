namespace BibliotecaMVC.Models
{
    public class Categoria
    {
        public int CategoriaID { get; set; }
        public required string Nombre { get; set; }
        public required string Descripcion { get; set; }

        // Relación con libros
        public ICollection<Libro>? Libros { get; set; } // Relación de navegación
    }
}
