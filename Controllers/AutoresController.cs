using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        private readonly BibliotecaDbContext _context;

        public AutoresController(BibliotecaDbContext context)
        {
            _context = context;
        }

        [Authorize] // Permitir acceso a todos los roles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Autores.ToListAsync());
        }

        [Authorize] // Permitir acceso a todos los roles
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var autor = await _context.Autores
                .Include(a => a.Libros) // Incluir libros relacionados con el autor
                .FirstOrDefaultAsync(a => a.AutorID == id);

            if (autor == null) return NotFound();

            return View(autor);
        }

        [Authorize(Roles = "Admin")] // Solo Admin puede crear
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Solo Admin puede crear
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Apellido,Nacionalidad,FechaNacimiento")] Autor autor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(autor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(autor);
        }

        [Authorize(Roles = "Admin")] // Solo Admin puede editar
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var autor = await _context.Autores.FindAsync(id);
            if (autor == null) return NotFound();

            return View(autor);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Solo Admin puede editar
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AutorID,Nombre,Apellido,Nacionalidad,FechaNacimiento")] Autor autor)
        {
            if (id != autor.AutorID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(autor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AutorExists(autor.AutorID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(autor);
        }

        [Authorize(Roles = "Admin")] // Solo Admin puede eliminar
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var autor = await _context.Autores.FirstOrDefaultAsync(a => a.AutorID == id);
            if (autor == null) return NotFound();

            return View(autor);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")] // Solo Admin puede eliminar
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var autor = await _context.Autores.FindAsync(id);
            if (autor != null)
            {
                _context.Autores.Remove(autor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AutorExists(int id)
        {
            return _context.Autores.Any(e => e.AutorID == id);
        }
    }
}
