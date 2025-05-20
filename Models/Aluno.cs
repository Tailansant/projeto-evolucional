using System.Collections.Generic;
using ProjetoEvolucional.Models;

namespace ProjetoEvolucional.Models
{
    public class Aluno
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;  // Diz ao compilador que não será null (mais perigoso)

    public ICollection<Nota> Notas { get; set; } = new List<Nota>();
}
}
