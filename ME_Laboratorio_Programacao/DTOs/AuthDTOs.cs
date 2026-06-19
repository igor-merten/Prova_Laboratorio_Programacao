using System.ComponentModel.DataAnnotations;

namespace Prova_Laboratorio_Programacao.DTOs;

public record LoginRequest(
    [Required(ErrorMessage = "O nome é obrigatório.")] 
    string Email,
    [Required(ErrorMessage = "O nome é obrigatório.")] 
    string Senha
);