using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BibliotecaMVC;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Controllers
{
    [Authorize(Roles = "Admin")] // Restringir acceso a Admin
    public class UsuariosController : Controller
    {
        private readonly BibliotecaDbContext _context;

        public UsuariosController(BibliotecaDbContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewBag.Roles = _context.Usuarios.Select(u => u.Rol).Distinct().ToList(); // Obtener roles únicos
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,NombreUsuario,Contraseña")] Usuario usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    usuario.Rol = "Usuario"; // Asignar el rol predeterminado
                    usuario.FechaRegistro = DateTime.Now; // Establecer la fecha de registro
                    _context.Add(usuario);

                    // Mensaje de depuración
                    Console.WriteLine($"Intentando crear usuario: Nombre={usuario.Nombre}, NombreUsuario={usuario.NombreUsuario}, Rol={usuario.Rol}");

                    await _context.SaveChangesAsync();

                    // Mensaje de éxito
                    ViewBag.Message = "Creación Exitosa";
                    ViewBag.IsSuccess = true;

                    // Limpiar el modelo para que los campos se vacíen
                    ModelState.Clear();
                    return View();
                }
                else
                {
                    Console.WriteLine("ModelState no es válido.");
                }
            }
            catch (Exception ex)
            {
                // Mensaje de error
                Console.WriteLine($"Error al crear usuario: {ex.Message}");
                ViewBag.Message = $"Error de creación (código de error: {ex.HResult})";
                ViewBag.IsSuccess = false;
            }

            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,NombreUsuario,Contraseña,Rol,FechaRegistro")] Usuario usuario)
        {
            if (id != usuario.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
