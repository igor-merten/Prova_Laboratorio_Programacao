namespace ME_Laboratorio_Programacao.Models;

public class EstatisticaAcesso : EntidadeBase
{
    public int AgenteId { get; set; }
    public int CanalOrigemId { get; set; }
    public int TotalSessoes { get; set; } = 0;
    public int TotalMensagens { get; set; } = 0;

    public virtual AgenteBase Agente { get; set; } = null!;
    public virtual CanalOrigem CanalOrigem { get; set; } = null!;
}
