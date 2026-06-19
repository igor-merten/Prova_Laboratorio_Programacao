using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Prova_Laboratorio_Programacao.DTOs;

public record UsuarioCreateRequest(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    string Nome,
    [Required(ErrorMessage = "O email é obrigatório.")] 
    string Email,
    [Required(ErrorMessage = "A senha é obrigatório.")]
    string Senha,
    [Required(ErrorMessage = "O perfil é obrigatório.")] 
    int PerfilAcessoId
);
public record UsuarioUpdateRequest(
    [Required(ErrorMessage = "O nome é obrigatório.")] 
    string Nome,
    [Required(ErrorMessage = "O status é obrigatório.")] 
    bool Ativo,
    [Required(ErrorMessage = "O perfil é obrigatório.")]
    int PerfilAcessoId,
    string? Senha
);