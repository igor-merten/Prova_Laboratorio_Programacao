namespace ME_Laboratorio_Programacao.Models.Interfaces;

public interface IAuditavel // A herança começa aqui, com a interface definindo que toda entidade auditavel tem uma data de criação
{
    DateTime DataCriacao { get; }
}
