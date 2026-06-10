namespace ME_Laboratorio_Programacao.Models.Mensagens;

public abstract class MensagemBase
{
    public int Id { get; set; }
    public int IdSessao { get; set; }
    public string Conteudo { get; set; } = "";
    public DateTime EnviadaEm { get; set; } = DateTime.UtcNow;

    public abstract string ExibirRemetente();
}
