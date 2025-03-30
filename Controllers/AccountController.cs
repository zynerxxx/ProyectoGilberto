using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BibliotecaMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly BibliotecaDbContext _context;

        public AccountController(BibliotecaDbContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == username && u.Contraseña == password);
            if (user != null)
            {
                if (string.IsNullOrEmpty(user.Nombre) || string.IsNullOrEmpty(user.Rol))
                {
                    ViewBag.Error = "El usuario no tiene un nombre o rol válido.";
                    return View();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.NombreUsuario),
                    new Claim(ClaimTypes.Role, user.Rol)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Usuarios");
            }

            ViewBag.Error = "Usuario o contraseña incorrectos.";
            return View();
        }

        // GET: Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login");
        }
    }
}
