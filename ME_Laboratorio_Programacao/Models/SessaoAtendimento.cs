namespace Prova_Laboratorio_Programacao.Models;

public class SessaoAtendimento : EntidadeBase // Classe SessaoAtendimento herdando da classe EntidadeBase
{
    public int UsuarioId { get; set; }
    public int AgenteId { get; set; }
    public int CanalOrigemId { get; set; }
    public DateTime DataInicio { get; set; } = DateTime.UtcNow;
    public DateTime? DataFim { get; set; }
    public string Status { get; set; } = "Aberta";

    // Navegações
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Agente Agente { get; set; } = null!;
    public virtual CanalOrigem CanalOrigem { get; set; } = null!;
    public virtual ICollection<Mensagem> Mensagens { get; set; } = new List<Mensagem>();
}
