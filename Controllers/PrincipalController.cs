using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoEvolucional.Data;
using ProjetoEvolucional.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoEvolucional.Controllers
{
    public class PrincipalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrincipalController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var usuarioLogado = HttpContext.Session.GetString("UsuarioLogado");

            if (string.IsNullOrEmpty(usuarioLogado))
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.UsuarioLogado = usuarioLogado;

            // Caso exista mensagem da ação anterior, mantém para exibir na view
            if (TempData["Mensagem"] != null)
            {
                ViewBag.Mensagem = TempData["Mensagem"]!.ToString();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Botao1()
        {
            var usuarioLogado = HttpContext.Session.GetString("UsuarioLogado");
            if (string.IsNullOrEmpty(usuarioLogado))
                return RedirectToAction("Login", "Auth");

            if (_context.Alunos.Any())
            {
                TempData["Mensagem"] = "Alunos já foram inseridos anteriormente.";
                return RedirectToAction("Index");
            }

            var disciplinas = new List<Disciplina>
            {
                new Disciplina { Nome = "Matemática" },
                new Disciplina { Nome = "Português" },
                new Disciplina { Nome = "História" },
                new Disciplina { Nome = "Geografia" },
                new Disciplina { Nome = "Inglês" },
                new Disciplina { Nome = "Biologia" },
                new Disciplina { Nome = "Filosofia" },
                new Disciplina { Nome = "Física" },
                new Disciplina { Nome = "Química" },
            };

            if (!_context.Disciplinas.Any())
            {
                _context.Disciplinas.AddRange(disciplinas);
                await _context.SaveChangesAsync();
            }
            else
            {
                disciplinas = _context.Disciplinas.ToList();
            }

            var alunos = new List<Aluno>();
            var notas = new List<Nota>();
            var random = new Random();

            for (int i = 1; i <= 1000; i++)
            {
                alunos.Add(new Aluno
                {
                    Nome = "Aluno" + i.ToString("D4")
                });
            }

            _context.Alunos.AddRange(alunos);
            await _context.SaveChangesAsync();

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
            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Inserção de alunos, disciplinas e notas concluída com sucesso.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Botao2()
        {
            var usuarioLogado = HttpContext.Session.GetString("UsuarioLogado");
            if (string.IsNullOrEmpty(usuarioLogado))
                return RedirectToAction("Login", "Auth");

            var alunos = await _context.Alunos
                .Include(a => a.Notas)
                .ThenInclude(n => n.Disciplina)
                .ToListAsync();

            if (!alunos.Any())
            {
                TempData["Mensagem"] = "Nenhum dado encontrado para gerar o relatório.";
                return RedirectToAction("Index");
            }

            var stream = new System.IO.MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Relatório");

                worksheet.Cells[1, 1].Value = "Aluno";
                int col = 2;

                var disciplinas = _context.Disciplinas.ToList();
                foreach (var d in disciplinas)
                {
                    worksheet.Cells[1, col].Value = d.Nome;
                    col++;
                }
                worksheet.Cells[1, col].Value = "Média";

                int row = 2;
                foreach (var aluno in alunos)
                {
                    worksheet.Cells[row, 1].Value = aluno.Nome;

                    double somaNotas = 0;
                    int qtdNotas = 0;

                    col = 2;
                    foreach (var d in disciplinas)
                    {
                        var nota = aluno.Notas.FirstOrDefault(n => n.DisciplinaId == d.Id);
                        if (nota != null)
                        {
                            worksheet.Cells[row, col].Value = nota.Valor;
                            somaNotas += nota.Valor;
                            qtdNotas++;
                        }
                        else
                        {
                            worksheet.Cells[row, col].Value = "-";
                        }
                        col++;
                    }

                    double media = qtdNotas > 0 ? somaNotas / qtdNotas : 0;
                    worksheet.Cells[row, col].Value = Math.Round(media, 2);

                    row++;
                }

                package.Save();
            }

            stream.Position = 0;
            string fileName = "Relatorio_Alunos_Notas.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}