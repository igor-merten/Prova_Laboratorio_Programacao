namespace ME_Laboratorio_Programacao.Models.Mensagens;

public class Mensagem
{
    public int Id { get; set; }
    public int SessaoAtendimentoId { get; set; }
    public string Remetente { get; set; } = "";  
    public string Conteudo { get; set; } = "";
    public DateTime EnviadaEm { get; set; }

    public virtual SessaoAtendimento SessaoAtendimento { get; set; } = null!;
    public MensagemBase ToMensagemBase(string nomeAgente = "Agente") =>
        Remetente == "usuario"
            ? new MensagemUsuario { Id = Id, SessaoAtendimentoId = SessaoAtendimentoId, Conteudo = Conteudo, EnviadaEm = EnviadaEm }
            : new MensagemAgente { Id = Id, SessaoAtendimentoId = SessaoAtendimentoId, Conteudo = Conteudo, EnviadaEm = EnviadaEm, NomeAgente = nomeAgente };
}