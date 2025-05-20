using Microsoft.AspNetCore.Mvc;
using ProjetoEvolucional.Data;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ProjetoEvolucional.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string login, string senha)
        {
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Login == login && u.Senha == senha);

            if (usuario == null)
            {
                ViewBag.Erro = "Usuário ou senha inválidos.";
                return View();
            }

            HttpContext.Session.SetString("UsuarioLogado", usuario.Login);
            return RedirectToAction("Index", "Principal");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
