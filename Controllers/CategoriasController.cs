using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly BibliotecaDbContext _context;

        public CategoriasController(BibliotecaDbContext context)
        {
            _context = context;
        }

        [Authorize] // Permitir acceso a todos los roles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categorias.ToListAsync());
        }

        [Authorize] // Permitir acceso a todos los roles
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var categoria = await _context.Categorias
                .Include(c => c.Libros) // Incluir libros relacionados con la categoría
                .FirstOrDefaultAsync(c => c.CategoriaID == id);

            if (categoria == null) return NotFound();

            return View(categoria);
        }

        [Authorize(Roles = "Admin")] // Solo Admin puede crear
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Solo Admin puede crear
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion")] Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        [Authorize(Roles = "Admin")] // Solo Admin puede editar
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();

            return View(categoria);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Solo Admin puede editar
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoriaID,Nombre,Descripcion")] Categoria categoria)
        {
            if (id != categoria.CategoriaID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.CategoriaID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        [Authorize(Roles = "Admin")] // Solo Admin puede eliminar
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.CategoriaID == id);
            if (categoria == null) return NotFound();

            return View(categoria);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")] // Solo Admin puede eliminar
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(e => e.CategoriaID == id);
        }
    }
}
