namespace Prova_Laboratorio_Programacao.Models;

public class PerfilAcesso : EntidadeBase // Classe PerfilAcesso herdando da classe EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;

    // Propriedade de navegação
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

}
