namespace ME_Laboratorio_Programacao.Models;

public class CanalOrigem : EntidadeBase
{
    public required string Nome { get; set; }
    public bool Ativo { get; set; } = true;

    public virtual ICollection<SessaoAtendimento> SessoesAtendimento { get; set; } = new List<SessaoAtendimento>();
    public virtual ICollection<EstatisticaAcesso> EstatisticasAcesso { get; set; } = new List<EstatisticaAcesso>();
}