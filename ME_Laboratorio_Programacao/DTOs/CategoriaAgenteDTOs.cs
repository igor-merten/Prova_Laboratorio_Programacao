using System.ComponentModel.DataAnnotations;

namespace ME_Laboratorio_Programacao.DTOs;

public record CategoriaRequest(

    [Required(ErrorMessage = "O nome é obrigatório.")] 
    string Nome,

    [Required(ErrorMessage = "A cor hexadecimal é obrigatória.")]
    string CorHex

);