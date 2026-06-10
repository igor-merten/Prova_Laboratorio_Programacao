namespace ME_Laboratorio_Programacao.Models;

public class Usuario : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    public int PerfilAcessoId { get; set; }

    // Propriedade de navegação
    public virtual PerfilAcesso? PerfilAcesso { get; set; }
}
