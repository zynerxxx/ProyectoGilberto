using System;

namespace BibliotecaMVC.Models
{
    public class Autor
    {
        public int AutorID { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public required string Nacionalidad { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }
}
