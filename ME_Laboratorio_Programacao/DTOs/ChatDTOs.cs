using System.ComponentModel.DataAnnotations;

namespace ME_Laboratorio_Programacao.DTOs;

public record IniciarSessaoRequest(
    [Required(ErrorMessage = "O Id de Agente é obrigatório.")]
    int AgenteId,
        
    [Required(ErrorMessage = "O Id de Canal de Origem é obrigatório.")]
    int CanalOrigemId
);

public record EnviarMensagemRequest(
    [Required(ErrorMessage = "O Id da Sessão é obrigatório.")]
    int SessaoId,

    [Required(ErrorMessage = "O conteúdo é obrigatório.")]
    string Conteudo
);
