using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ME_Laboratorio_Programacao.Data;
using ME_Laboratorio_Programacao.Models;
using ME_Laboratorio_Programacao.DTOs;
using ME_Laboratorio_Programacao.Simuladores;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading.Tasks;
using ME_Laboratorio_Programacao.Models.Mensagens;

namespace ME_Laboratorio_Programacao.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChatController(AppDbContext context){
            _context = context;
        }

        private int GetUsuarioId() {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        [HttpPost("iniciar")]
        public async Task<IActionResult> IniciarSessao([FromBody] IniciarSessaoRequest request){
            var usuarioId = GetUsuarioId();
            
            var sessaoAberta = await _context.SessoesAtendimento
                .FirstOrDefaultAsync(s => s.UsuarioId == usuarioId && s.AgenteId == request.AgenteId && s.Status == "Aberta");
                
            if (sessaoAberta != null)
                return Ok(new { sessaoId = sessaoAberta.Id });

            var novaSessao = new SessaoAtendimento {
                UsuarioId = usuarioId,
                AgenteId = request.AgenteId,
                CanalOrigemId = request.CanalOrigemId,
                Status = "Aberta",
                DataInicio = DateTime.UtcNow
            };

            _context.SessoesAtendimento.Add(novaSessao);
            await _context.SaveChangesAsync();

            return Ok(new { sessaoId = novaSessao.Id });
        }

        [HttpGet("sessoes")]
        public async Task<IActionResult> ListarSessoes(){
            var usuarioId = GetUsuarioId();
            var sessoes = await _context.SessoesAtendimento
                .Include(s => s.Agente)
                .Include(s => s.CanalOrigem)
                .Where(s => s.UsuarioId == usuarioId)
                .Select(s => new {
                    s.Id,
                    s.Status,
                    s.DataInicio,
                    AgenteNome = s.Agente.Nome,
                    CanalNome = s.CanalOrigem.Nome
                })
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            return Ok(sessoes);
        }

        [HttpGet("{sessaoId}/mensagens")]
        public async Task<IActionResult> ObterMensagens(int sessaoId){
            var usuarioId = GetUsuarioId();
            var sessao = await _context.SessoesAtendimento.FirstOrDefaultAsync(s => s.Id == sessaoId && s.UsuarioId == usuarioId);
            if(sessao == null) return Forbid();

            var mensagens = await _context.Mensagens
                .Where(m => m.SessaoAtendimentoId == sessaoId)
                .OrderBy(m => m.EnviadaEm)
                .Select(m => new {
                    m.Id,
                    m.Remetente,
                    m.Conteudo,
                    m.EnviadaEm
                })
                .ToListAsync();

            return Ok(mensagens);
        }

        [HttpPost("enviar")]
        public async Task<IActionResult> EnviarMensagem([FromBody] EnviarMensagemRequest request){
            var usuarioId = GetUsuarioId();
            var sessao = await _context.SessoesAtendimento
                .Include(s => s.Agente)
                .FirstOrDefaultAsync(s => s.Id == request.SessaoId && s.UsuarioId == usuarioId);

            if (sessao == null) return NotFound("Sessão não encontrada");

            var msgUsuario = new Mensagem{
                SessaoAtendimentoId = sessao.Id,
                Remetente = "usuario",
                Conteudo = request.Conteudo,
                EnviadaEm = DateTime.UtcNow
            };
            _context.Mensagens.Add(msgUsuario);

            AgenteSimulador motor;
            switch (sessao.Agente.CategoriaAgenteId){
                case 1:
                    motor = new SimuladorVendas();
                    break;
                case 2:
                    motor = new SimuladorSuporte();
                    break;
                case 4:
                    motor = new SimuladorRH();
                    break;
                default:
                    motor = new SimuladorPadrao();
                    break;
            }

            string respostaGerada = motor.GerarResposta(request.Conteudo);

            var msgAgente = new Mensagem{
                SessaoAtendimentoId = sessao.Id,
                Remetente = "Agente",
                Conteudo = respostaGerada,
                EnviadaEm = DateTime.UtcNow
            };
            _context.Mensagens.Add(msgAgente);

            var contexto = await _context.ContextosMemoria
                .FirstOrDefaultAsync(c => c.UsuarioId == sessao.UsuarioId && c.AgenteId == sessao.AgenteId);
            
            if (contexto == null){
                contexto = new ContextoMemoria{
                    UsuarioId = sessao.UsuarioId,
                    AgenteId = sessao.AgenteId,
                    Resumo = "Último assunto: " + request.Conteudo,
                    UltimaAtualizacao = DateTime.UtcNow
                };
                _context.ContextosMemoria.Add(contexto);
            }else{
                contexto.Resumo = "Último assunto: " + request.Conteudo;
                contexto.UltimaAtualizacao = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new { mensagem = respostaGerada, remetente = "Agente" });
        }
    }
}
