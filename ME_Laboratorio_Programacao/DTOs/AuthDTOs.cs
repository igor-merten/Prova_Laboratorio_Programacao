namespace ME_Laboratorio_Programacao.DTOs;

public record LoginRequest(string Email, string Senha);
public record UsuarioCreateRequest(string Nome, string Email, string Senha, int PerfilAcessoId);
public record UsuarioUpdateRequest(string Nome, string Email, bool Ativo, int PerfilAcessoId);
public record PerfilAcessoRequest(string Nome, string Descricao);