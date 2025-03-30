using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly BibliotecaDbContext _context;

        public LibrosController(BibliotecaDbContext context)
        {
            _context = context;
        }

        [Authorize] // Permitir acceso a todos los roles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Libros.ToListAsync());
        }

        [Authorize] // Permitir acceso a todos los roles
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var libro = await _context.Libros
                .Include(l => l.Autor) // Incluir información del autor
                .Include(l => l.Categoria) // Incluir información de la categoría
                .FirstOrDefaultAsync(m => m.LibroID == id);

            if (libro == null) return NotFound();

            return View(libro);
        }

        [Authorize(Roles = "Admin")] // Solo Admin puede crear
        public IActionResult Create()
        {
            ViewBag.Autores = _context.Autores.ToList(); // Obtener autores para el menú desplegable
            ViewBag.Categorias = _context.Categorias.ToList(); // Obtener categorías para el menú desplegable
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Solo Admin puede crear
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titulo,ISBN,Editorial,NumeroCopias,AutorID,CategoriaID")] Libro libro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(libro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Autores = _context.Autores.ToList(); // Reestablecer autores si hay error
            ViewBag.Categorias = _context.Categorias.ToList(); // Reestablecer categorías si hay error
            return View(libro);
        }

        [Authorize(Roles = "Admin")] // Solo Admin puede editar
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var libro = await _context.Libros.FindAsync(id);
            if (libro == null) return NotFound();

            ViewBag.Autores = _context.Autores.ToList(); // Obtener autores para el menú desplegable
            ViewBag.Categorias = _context.Categorias.ToList(); // Obtener categorías para el menú desplegable
            return View(libro);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Solo Admin puede editar
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LibroID,Titulo,ISBN,Editorial,NumeroCopias,AutorID,CategoriaID")] Libro libro)
        {
            if (id != libro.LibroID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(libro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibroExists(libro.LibroID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Autores = _context.Autores.ToList(); // Reestablecer autores si hay error
            ViewBag.Categorias = _context.Categorias.ToList(); // Reestablecer categorías si hay error
            return View(libro);
        }

        [Authorize(Roles = "Admin")] // Solo Admin puede eliminar
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var libro = await _context.Libros.FirstOrDefaultAsync(m => m.LibroID == id);
            if (libro == null) return NotFound();

            return View(libro);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")] // Solo Admin puede eliminar
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro != null)
            {
                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LibroExists(int id)
        {
            return _context.Libros.Any(e => e.LibroID == id);
        }
    }
}
