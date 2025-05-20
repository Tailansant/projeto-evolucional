// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using ProjetoEvolucional.Models;

namespace ProjetoEvolucional.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Nota> Notas { get; set; }
        public DbSet<Disciplina> Disciplinas { get; set; }
    }
}
