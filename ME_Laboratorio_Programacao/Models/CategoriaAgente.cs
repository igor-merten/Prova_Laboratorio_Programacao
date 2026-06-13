namespace ME_Laboratorio_Programacao.Models;

public class CategoriaAgente
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string CorHex { get; set; } = "#ffffff";

    public virtual ICollection<Agente> Agentes { get; set; } = new List<Agente>();
}
