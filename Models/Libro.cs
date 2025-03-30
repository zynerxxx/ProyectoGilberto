namespace BibliotecaMVC.Models
{
    public class Libro
    {
        public int LibroID { get; set; }
        public required string Titulo { get; set; }
        public int AutorID { get; set; }
        public required string ISBN { get; set; }
        public int? AnioPublicacion { get; set; }
        public required string Editorial { get; set; }
        public int NumeroCopias { get; set; }
        public int CategoriaID { get; set; }

        // Relación con Autor
        public Autor? Autor { get; set; } // Declarar como anulable

        // Relación con Categoría
        public Categoria? Categoria { get; set; } // Declarar como anulable
    }
}
