using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotecaMVC.Models
{
    public class Usuario
    {
        [Column("UsuarioID")]
        public int Id { get; set; }

        [Column("Nombre")]
        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        [Column("Usuario")]
        [Required(ErrorMessage = "El campo Nombre de Usuario es obligatorio.")]
        public string NombreUsuario { get; set; } = string.Empty; // Cambiar el nombre de la propiedad

        [Column("Contraseña")]
        [Required(ErrorMessage = "El campo Contraseña es obligatorio.")]
        public string Contraseña { get; set; } = string.Empty;

        [Column("Rol")]
        [Required(ErrorMessage = "El campo Rol es obligatorio.")]
        public string Rol { get; set; } = "Usuario";

        [Column("FechaRegistro")]
        public DateTime FechaRegistro { get; set; }
    }
}
