using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Controllers
{
    public class PrestamosController : Controller
    {
        private readonly BibliotecaDbContext _context;

        public PrestamosController(BibliotecaDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var prestamos = await _context.Prestamos
                .Include(p => p.Libro) // Incluir información del libro
                .Where(p => p.Estado == "Pendiente") // Filtrar solo los préstamos pendientes
                .ToListAsync();

            return View(prestamos);
        }

        [Authorize]
        public IActionResult Create(int libroId, string? returnUrl = null)
        {
            var libro = _context.Libros.FirstOrDefault(l => l.LibroID == libroId);
            if (libro == null) return NotFound();

            if (libro.NumeroCopias <= 0)
            {
                TempData["Error"] = "No hay copias disponibles para este libro.";
                return RedirectToAction("Details", "Libros", new { id = libroId });
            }

            ViewBag.LibroTitulo = libro.Titulo;

            // Validar y decodificar returnUrl
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                ViewBag.ReturnUrl = returnUrl;
            }
            else
            {
                ViewBag.ReturnUrl = Url.Action("Index", "Libros");
            }

            // Si el usuario es Admin, cargar la lista de usuarios
            if (User.IsInRole("Admin"))
            {
                ViewBag.Usuarios = _context.Usuarios
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = $"{u.Nombre} ({u.NombreUsuario})"
                    })
                    .ToList();
            }

            return View(new Prestamo { LibroID = libroId });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LibroID,FechaDevolucion,UsuarioID")] Prestamo prestamo, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var libro = _context.Libros.FirstOrDefault(l => l.LibroID == prestamo.LibroID);
                if (libro == null || libro.NumeroCopias <= 0)
                {
                    ModelState.AddModelError(string.Empty, "No hay copias disponibles para este libro.");
                    return View(prestamo);
                }

                prestamo.FechaPrestamo = DateTime.Now;
                prestamo.Estado = "Pendiente";

                // Si el usuario no es Admin, asignar el UsuarioID del usuario autenticado
                if (!User.IsInRole("Admin"))
                {
                    if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
                    {
                        ModelState.AddModelError(string.Empty, "No se pudo identificar al usuario.");
                        return View(prestamo);
                    }

                    var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == User.Identity.Name);
                    if (usuario == null)
                    {
                        ModelState.AddModelError(string.Empty, $"El usuario '{User.Identity.Name}' no existe en el sistema.");
                        return View(prestamo);
                    }
                    prestamo.UsuarioID = usuario.Id;
                }

                _context.Add(prestamo);

                // Reducir el número de copias disponibles
                libro.NumeroCopias -= 1;
                _context.Update(libro);

                await _context.SaveChangesAsync();

                TempData["Success"] = "El préstamo se ha realizado correctamente.";

                // Redirigir a la página de Préstamos
                return RedirectToAction("Index", "Prestamos");
            }

            // Si hay un error, recargar la lista de usuarios para Admin
            if (User.IsInRole("Admin"))
            {
                ViewBag.Usuarios = _context.Usuarios
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = $"{u.Nombre} ({u.NombreUsuario})"
                    })
                    .ToList();
            }

            ViewBag.ReturnUrl = returnUrl; // Pasar la URL de retorno a la vista
            return View(prestamo);
        }

        [Authorize(Roles = "Admin")] // Solo Admin puede editar
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var prestamo = await _context.Prestamos.FindAsync(id);
            if (prestamo == null) return NotFound();

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "Devuelto", Text = "Devuelto" }
            };

            return View(prestamo);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Solo Admin puede editar
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PrestamoID,LibroID,FechaPrestamo,FechaDevolucion,Estado")] Prestamo prestamo)
        {
            if (id != prestamo.PrestamoID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prestamo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Prestamos.Any(e => e.PrestamoID == prestamo.PrestamoID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(prestamo);
        }

        [Authorize(Roles = "Admin")] // Solo Admin puede eliminar
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var prestamo = await _context.Prestamos
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(m => m.PrestamoID == id);
            if (prestamo == null) return NotFound();

            return View(prestamo);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")] // Solo Admin puede eliminar
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prestamo = await _context.Prestamos.FindAsync(id);
            if (prestamo != null)
            {
                _context.Prestamos.Remove(prestamo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Solo Admin puede marcar como devuelto
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsReturned(int id)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.Libro) // Incluir información del libro para actualizar copias
                .FirstOrDefaultAsync(p => p.PrestamoID == id);

            if (prestamo == null) return NotFound();

            if (prestamo.Estado == "Devuelto")
            {
                TempData["Error"] = "El préstamo ya está marcado como devuelto.";
                return RedirectToAction(nameof(Index));
            }

            // Marcar como devuelto
            prestamo.Estado = "Devuelto";
            prestamo.FechaDevuelto = DateTime.Now; // Registrar la fecha de devolución real

            // Incrementar el número de copias disponibles del libro
            if (prestamo.Libro != null)
            {
                prestamo.Libro.NumeroCopias += 1;
                _context.Update(prestamo.Libro);
            }

            _context.Update(prestamo);
            await _context.SaveChangesAsync();

            TempData["Success"] = "El préstamo se ha marcado como devuelto.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")] // Solo Admin puede ver el historial
        public async Task<IActionResult> Historial()
        {
            var prestamos = await _context.Prestamos
                .Include(p => p.Libro) // Incluir información del libro
                .Include(p => p.Usuario) // Incluir información del usuario
                .ToListAsync(); // Asegurarse de cargar las relaciones

            return View(prestamos);
        }
    }
}
