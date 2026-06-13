using ME_Laboratorio_Programacao.Data;
using ME_Laboratorio_Programacao.DTOs;
using ME_Laboratorio_Programacao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ME_Laboratorio_Programacao.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AgentesController : ControllerBase
{
    private readonly AppDbContext _context;

    public AgentesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ListarAgentes()
    {
        return Ok(await _context.Agentes.Include(a => a.CategoriaAgente).ToListAsync());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CriarAgente([FromBody] AgenteRequest request)
    {
        Agente novoAgente = new Agente { 
                Nome = request.Nome,
                CategoriaAgenteId = request.CategoriaAgenteId,
                Descricao = request.Descricao,
                Ativo = request.Ativo
            }; 

        _context.Agentes.Add(novoAgente);
        await _context.SaveChangesAsync(); // A auditoria vai identificar se salvou SuperAgente ou AgentePadrao!

        return CreatedAtAction(nameof(ListarAgentes), new { id = novoAgente.Id }, novoAgente);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AtualizarAgente(int id, [FromBody] AgenteRequest request)
    {
        var agenteBanco = await _context.Agentes.FindAsync(id);
        if (agenteBanco == null) return NotFound();

        agenteBanco.Nome = request.Nome;
        agenteBanco.CategoriaAgenteId = request.CategoriaAgenteId;
        agenteBanco.Descricao = request.Descricao;
        agenteBanco.Ativo = request.Ativo;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletarCanal(int id)
    {
        var agente = await _context.Agentes.FindAsync(id);
        if (agente == null) return NotFound();

        _context.Agentes.Remove(agente);
        await _context.SaveChangesAsync();
        return NoContent();
    }

}
