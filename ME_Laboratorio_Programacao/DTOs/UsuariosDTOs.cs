namespace ME_Laboratorio_Programacao.DTOs;

public record UsuarioCreateRequest(string Nome, string Email, string Senha, int PerfilAcessoId);
public record UsuarioUpdateRequest(string Nome, bool Ativo, string? Senha, int PerfilAcessoId);