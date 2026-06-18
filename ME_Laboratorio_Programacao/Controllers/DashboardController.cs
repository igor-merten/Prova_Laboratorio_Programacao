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
        var totalSessoes = await _context.EstatisticasAcesso.SumAsync(e => e.TotalSessoes);
        var totalMensagens = await _context.EstatisticasAcesso.SumAsync(e => e.TotalMensagens);

        var mensagensPorAgente = await _context.EstatisticasAcesso
        .Include(e => e.Agente)
        .GroupBy(e => e.Agente.Nome)
        .Select(g => new { Agente = g.Key, Total = g.Sum(e => e.TotalMensagens) })
        .ToListAsync();

        var sessoesPorCanal = await _context.EstatisticasAcesso
        .Include(e => e.CanalOrigem)
        .GroupBy(e => e.CanalOrigem.Nome)
        .Select(g => new { Canal = g.Key, Total = g.Sum(e => e.TotalSessoes) })
        .ToListAsync();

        var sessoesPorAgente = await _context.EstatisticasAcesso
        .Include(e => e.Agente)
        .GroupBy(e => e.Agente.Nome)
        .Select(g => new { Agente = g.Key, Total = g.Sum(e => e.TotalSessoes) })
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
