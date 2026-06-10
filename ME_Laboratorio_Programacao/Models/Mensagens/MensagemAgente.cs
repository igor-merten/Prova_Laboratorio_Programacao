namespace ME_Laboratorio_Programacao.Models.Mensagens;

public class MensagemAgente : MensagemBase
{
    public string NomeAgente { get; set; } = "Agente";
    public override string ExibirRemetente() => NomeAgente;
}