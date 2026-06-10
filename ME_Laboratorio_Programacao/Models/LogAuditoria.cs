namespace ME_Laboratorio_Programacao.Models;

public class LogAuditoria : EntidadeBase
{
    public int? UsuarioId { get; set; } // Nullable devido ao ON DELETE SET NULL
    public required string Acao { get; set; }
    public string? Entidade { get; set; }
    public int? EntidadeId { get; set; }
    public string? IpOrigem { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
