namespace ME_Laboratorio_Programacao.DTOs;

public record LoginRequest(string Email, string Senha);
public record PerfilAcessoRequest(string Nome, string Descricao);