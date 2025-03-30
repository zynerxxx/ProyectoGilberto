using System;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class Prestamo
    {
        public int PrestamoID { get; set; }
        public int LibroID { get; set; } // Relación con el libro
        public int UsuarioID { get; set; } // Relación con el usuario

        public DateTime FechaPrestamo { get; set; }

        [DataType(DataType.Date)]
        [CustomValidation(typeof(Prestamo), nameof(ValidarFechaDevolucion))]
        public DateTime? FechaDevolucion { get; set; }

        public DateTime? FechaDevuelto { get; set; } // Nueva columna para la fecha de devolución real

        public string Estado { get; set; } = "Pendiente"; // Valor predeterminado para Estado

        // Relación con Libro
        public Libro? Libro { get; set; }

        // Relación con Usuario
        public Usuario? Usuario { get; set; }

        // Validación personalizada para la fecha de devolución
        public static ValidationResult? ValidarFechaDevolucion(DateTime? fechaDevolucion, ValidationContext context)
        {
            if (fechaDevolucion == null)
            {
                return new ValidationResult("La fecha de devolución es obligatoria.");
            }

            if (fechaDevolucion < DateTime.Today)
            {
                return new ValidationResult("La fecha de devolución no puede ser anterior a la fecha actual.");
            }

            return ValidationResult.Success;
        }
    }
}
