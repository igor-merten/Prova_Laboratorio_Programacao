using System.ComponentModel.DataAnnotations;

namespace Prova_Laboratorio_Programacao.DTOs;

public record CanalRequest(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    string Nome,
    [Required(ErrorMessage = "O status é obrigatório.")]
    bool Ativo
);