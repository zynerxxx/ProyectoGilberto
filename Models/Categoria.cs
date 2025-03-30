namespace BibliotecaMVC.Models
{
    public class Categoria
    {
        public int CategoriaID { get; set; }
        public required string Nombre { get; set; }
        public required string Descripcion { get; set; }
    }
}
