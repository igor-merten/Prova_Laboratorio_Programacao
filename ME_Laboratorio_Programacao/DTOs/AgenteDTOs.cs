using System.ComponentModel.DataAnnotations;

namespace Prova_Laboratorio_Programacao.DTOs;


public record AgenteRequest(
    [Required(ErrorMessage = "O nome é obrigatório.")] 
    string Nome,

    [Required(ErrorMessage = "A descrição é obrigatória.")] 
    string Descricao,

    [Required(ErrorMessage = "A categoria do agente é obrigatória.")]
    int CategoriaAgenteId,

    [Required(ErrorMessage = "O status é obrigatório.")]
    bool Ativo
);
