using Prova_Laboratorio_Programacao.Models;

namespace Prova_Laboratorio_Programacao.Models;

public class Mensagem
{
    public int Id { get; set; }
    public int SessaoAtendimentoId { get; set; }
    public string Remetente { get; set; } = "";  
    public string Conteudo { get; set; } = "";
    public DateTime EnviadaEm { get; set; }

    public virtual SessaoAtendimento SessaoAtendimento { get; set; } = null!;
}