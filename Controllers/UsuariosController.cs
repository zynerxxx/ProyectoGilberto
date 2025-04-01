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
        [AllowAnonymous] // Permitir acceso sin autenticación
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [AllowAnonymous] // Permitir acceso sin autenticación
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,NombreUsuario,Contraseña,Rol")] Usuario usuario)
        {
            if (_context.Usuarios.Any(u => u.NombreUsuario == usuario.NombreUsuario))
            {
                ModelState.AddModelError("NombreUsuario", "El nombre de usuario ya está en uso. Por favor, elige otro.");
                return View(usuario);
            }

            if (ModelState.IsValid)
            {
                usuario.FechaRegistro = DateTime.Now;
                usuario.Rol = "Usuario"; // Asignar el rol predeterminado
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Usuario creado exitosamente. Ahora puedes iniciar sesión.";
                return RedirectToAction("Login", "Account");
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
