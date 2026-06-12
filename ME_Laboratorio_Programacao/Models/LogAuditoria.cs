namespace ME_Laboratorio_Programacao.Models;

public class LogAuditoria : EntidadeBase
{
    public int? UsuarioId { get; set; }
    public required string Acao { get; set; }
    public string? Entidade { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
