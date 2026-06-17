using System.ComponentModel.DataAnnotations;

namespace ME_Laboratorio_Programacao.DTOs;

public record CanalRequest(
    [Required(ErrorMessage = "O nome do canal é obrigatório.")]
    string Nome, 
    bool Ativo
);