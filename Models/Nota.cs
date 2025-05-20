using ProjetoEvolucional.Models;
namespace ProjetoEvolucional.Models;


public class Nota
{
    public int Id { get; set; }
    public int AlunoId { get; set; }
    public Aluno Aluno { get; set; } = null!;
    public int DisciplinaId { get; set; }
    public Disciplina Disciplina { get; set; } = null!;
    public double Valor { get; set; }
}
