using Prova_Laboratorio_Programacao.Models.Interfaces;

namespace Prova_Laboratorio_Programacao.Models;

public abstract class EntidadeBase : IAuditavel // Aqui a herança continuando, a classe base herdando da interface IAuditavel
{
    public int Id { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

}
