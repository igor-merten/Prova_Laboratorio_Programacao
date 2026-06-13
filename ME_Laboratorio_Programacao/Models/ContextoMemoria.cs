namespace ME_Laboratorio_Programacao.Models;

public class ContextoMemoria
{
    public int Id { get; set; }
    public int AgenteId { get; set; }
    public int UsuarioId { get; set; }
    public string? Resumo { get; set; }
    public DateTime UltimaAtualizacao { get; set; } = DateTime.UtcNow;

    public virtual Agente Agente { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
}