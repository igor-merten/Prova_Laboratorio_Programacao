using ME_Laboratorio_Programacao.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ME_Laboratorio_Programacao.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context){
        _context = context;
    }

    [HttpGet("estatisticas")]
    public async Task<IActionResult> ObterEstatisticas() {
        var totalSessoes = await _context.SessoesAtendimento.CountAsync();
        var totalMensagens = await _context.Mensagens.CountAsync();
        var mensagensPorAgente = await _context.Mensagens
            .Include(m => m.SessaoAtendimento)
            .ThenInclude(s => s.Agente)
            .GroupBy(m => m.SessaoAtendimento.Agente.Nome)
            .Select(g => new { Agente = g.Key, Total = g.Count() })
            .ToListAsync();

        var sessoesPorCanal = await _context.SessoesAtendimento
            .Include(s => s.CanalOrigem)
            .GroupBy(s => s.CanalOrigem.Nome)
            .Select(g => new { Canal = g.Key, Total = g.Count() })
            .ToListAsync();

        var sessoesPorAgente = await _context.SessoesAtendimento
            .Include(s => s.Agente)
            .GroupBy(s => s.Agente.Nome)
            .Select(g => new { Agente = g.Key, Total = g.Count() })
            .ToListAsync();

        return Ok(new {
            TotalSessoes = totalSessoes,
            TotalMensagens = totalMensagens,
            MensagensPorAgente = mensagensPorAgente,
            SessoesPorCanal = sessoesPorCanal,
            SessoesPorAgente = sessoesPorAgente
        });
    }
}
