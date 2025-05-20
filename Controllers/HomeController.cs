using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjetoEvolucional.Data;
using ProjetoEvolucional.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjetoEvolucional.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult InserirDados()
        {
            if (_context.Alunos.Any())
            {
                ViewBag.Mensagem = "Dados já inseridos.";
                return View("Index");
            }

            // Inserir algumas disciplinas
            var disciplinas = new List<Disciplina>
            {
                new Disciplina { Nome = "Matemática" },
                new Disciplina { Nome = "Português" },
                new Disciplina { Nome = "História" }
            };

            _context.Disciplinas.AddRange(disciplinas);
            _context.SaveChanges();

            // Inserir alguns alunos
            var alunos = new List<Aluno>
            {
                new Aluno { Nome = "João Silva" },
                new Aluno { Nome = "Maria Oliveira" }
            };
            _context.Alunos.AddRange(alunos);
            _context.SaveChanges();

            // Inserir notas para os alunos nas disciplinas
            var random = new Random();
            var notas = new List<Nota>();

            foreach (var aluno in alunos)
            {
                foreach (var disciplina in disciplinas)
                {
                    notas.Add(new Nota
                    {
                        AlunoId = aluno.Id,
                        DisciplinaId = disciplina.Id,
                        Valor = Math.Round(random.NextDouble() * 10, 2)
                    });
                }
            }

            _context.Notas.AddRange(notas);
            _context.SaveChanges();

            ViewBag.Mensagem = "Dados inseridos com sucesso!";
            return View("Index");
        }

        [HttpPost]
        public IActionResult GerarRelatorio()
        {
            // Carregar alunos com as notas e disciplinas (para mostrar no relatório)
            var alunos = _context.Alunos
                .Select(a => new
                {
                    a.Nome,
                    Notas = a.Notas.Select(n => new {
                        Disciplina = n.Disciplina.Nome,
                        Nota = n.Valor
                    }).ToList()
                })
                .ToList();

            return View("Relatorio", alunos);
        }
    }
}