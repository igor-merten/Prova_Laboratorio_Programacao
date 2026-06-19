using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prova_Laboratorio_Programacao.Data;
using Prova_Laboratorio_Programacao.Models;
using Prova_Laboratorio_Programacao.DTOs;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Prova_Laboratorio_Programacao.Controllers
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
            
            var agente = await _context.Agentes.FindAsync(request.AgenteId);
            if (agente == null || !agente.Ativo) return BadRequest("Agente inativo ou não encontrado.");

            var canal = await _context.CanaisOrigem.FindAsync(request.CanalOrigemId);
            if (canal == null || !canal.Ativo) return BadRequest("Canal inativo ou não encontrado.");

            var sessaoAberta = await _context.SessoesAtendimento
                .FirstOrDefaultAsync(s => s.UsuarioId == usuarioId && s.AgenteId == request.AgenteId && s.CanalOrigemId == request.CanalOrigemId && s.Status == "Aberta");
                
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

            var estatistica = await _context.EstatisticasAcesso
                .FirstOrDefaultAsync(e => e.AgenteId == request.AgenteId && e.CanalOrigemId == request.CanalOrigemId);
            if (estatistica == null)
            {
                _context.EstatisticasAcesso.Add(new EstatisticaAcesso {
                    AgenteId = request.AgenteId,
                    CanalOrigemId = request.CanalOrigemId,
                    TotalSessoes = 1,
                    TotalMensagens = 0
                });
            }
            else
            {
                estatistica.TotalSessoes += 1;
            }

            var memoria = await _context.ContextosMemoria
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.AgenteId == request.AgenteId);

            if (memoria != null && !string.IsNullOrEmpty(memoria.Resumo))
            {
                var msgMemoria = new Mensagem{
                    SessaoAtendimentoId = novaSessao.Id,
                    Remetente = "Agente",
                    Conteudo = $"[Memória do Agente] Olá novamente! De acordo com meus registros, nosso último assunto foi: {memoria.Resumo}. Como posso continuar te ajudando hoje?",
                    EnviadaEm = DateTime.UtcNow
                };
                _context.Mensagens.Add(msgMemoria);
            }
            
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
                .Include(s => s.CanalOrigem)
                .FirstOrDefaultAsync(s => s.Id == request.SessaoId && s.UsuarioId == usuarioId);

            if (sessao == null) return NotFound("Sessão não encontrada.");
            if (!sessao.Agente.Ativo) return BadRequest("Este agente foi inativado e não pode mais receber ou enviar mensagens.");
            if (!sessao.CanalOrigem.Ativo) return BadRequest("O canal desta sessão foi inativado.");

            var msgUsuario = new Mensagem{
                SessaoAtendimentoId = sessao.Id,
                Remetente = "usuario",
                Conteudo = request.Conteudo,
                EnviadaEm = DateTime.UtcNow
            };
            _context.Mensagens.Add(msgUsuario);

            // POLIMORFISMO E HERANÇA
            AgenteSimulador motor; // Variavel que vai receber qualquer agenteSimulador
            switch (sessao.Agente.CategoriaAgenteId){
                case 1:
                    motor = new SimuladorVendas(); // Recebe SimuladorVendas
                    break;
                case 2:
                    motor = new SimuladorSuporte(); // Recebe SimuladorSuporte
                    break;
                case 3:
                    motor = new SimuladorFinanceiro(); // Recebe SimuladorFinanceiro
                    break;
                case 4:
                    motor = new SimuladorRH(); // Recebe SimuladorRH
                    break;
                default:
                    motor = new SimuladorPadrao(); // Recebe SimuladorPadrao
                    break;
            }

            // E aqui chama o método certo de cada subclasse
            string respostaGerada = motor.GerarResposta(request.Conteudo);

            var msgAgente = new Mensagem{
                SessaoAtendimentoId = sessao.Id,
                Remetente = "Agente",
                Conteudo = respostaGerada,
                EnviadaEm = DateTime.UtcNow
            };
            _context.Mensagens.Add(msgAgente);

            var estatistica = await _context.EstatisticasAcesso
                .FirstOrDefaultAsync(e => e.AgenteId == sessao.AgenteId && e.CanalOrigemId == sessao.CanalOrigemId);
            if (estatistica != null)
            {
                estatistica.TotalMensagens += 2;
            }

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
        [HttpDelete("{sessaoId}")]
        public async Task<IActionResult> DeletarSessao(int sessaoId)
        {
            var usuarioId = GetUsuarioId();
            var sessao = await _context.SessoesAtendimento
                .FirstOrDefaultAsync(s => s.Id == sessaoId && s.UsuarioId == usuarioId);

            if (sessao == null) return NotFound("Sessão não encontrada");

            var memoria = await _context.ContextosMemoria
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.AgenteId == sessao.AgenteId);
            
            if (memoria != null) {
                _context.ContextosMemoria.Remove(memoria);
            }

            var qtdMensagens = await _context.Mensagens.CountAsync(m => m.SessaoAtendimentoId == sessaoId);
            var estatistica = await _context.EstatisticasAcesso
                .FirstOrDefaultAsync(e => e.AgenteId == sessao.AgenteId && e.CanalOrigemId == sessao.CanalOrigemId);
            
            if (estatistica != null)
            {
                estatistica.TotalSessoes -= 1;
                estatistica.TotalMensagens -= qtdMensagens;
                if (estatistica.TotalSessoes < 0) estatistica.TotalSessoes = 0;
                if (estatistica.TotalMensagens < 0) estatistica.TotalMensagens = 0;
            }

            _context.SessoesAtendimento.Remove(sessao);
            await _context.SaveChangesAsync();
            
            return Ok("Sessão e histórico deletados com sucesso!");
        }
    }
}
