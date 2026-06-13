using System.ComponentModel.DataAnnotations;

namespace ME_Laboratorio_Programacao.DTOs
{
    public class IniciarSessaoRequest{
        [Required]
        public int AgenteId { get; set; }
            
        [Required]
        public int CanalOrigemId { get; set; }
    }

    public class EnviarMensagemRequest {
        [Required]
        public int SessaoId { get; set; }
        [Required]
        public string Conteudo { get; set; }
    }
}
