using ME_Laboratorio_Programacao.Models;

public abstract class AgenteBase : EntidadeBase
{
    public required virtual string Nome { get; set; }
    public string? Descricao { get; set; }
    public int CategoriaAgenteId { get; set; }
    public bool Ativo { get; set; } = true;

    // Propriedades de navegação
    public virtual CategoriaAgente CategoriaAgente { get; set; } = null!;
    public virtual ICollection<SessaoAtendimento> SessoesAtendimento { get; set; } = new List<SessaoAtendimento>();
    public virtual ICollection<ContextoMemoria> ContextosMemoria { get; set; } = new List<ContextoMemoria>();
    public virtual ICollection<EstatisticaAcesso> EstatisticasAcesso { get; set; } = new List<EstatisticaAcesso>();
}