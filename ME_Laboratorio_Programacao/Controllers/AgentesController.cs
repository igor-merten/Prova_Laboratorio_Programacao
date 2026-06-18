using ME_Laboratorio_Programacao.Data;
using ME_Laboratorio_Programacao.DTOs;
using ME_Laboratorio_Programacao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace ME_Laboratorio_Programacao.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AgentesController : ControllerBase
{
    private readonly AppDbContext _context;

    public AgentesController(AppDbContext context)
    {
        _context = context;
    }
    private int GetUsuarioId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<IActionResult> ListarAgentes()
    {
        return Ok(await _context.Agentes.Include(a => a.CategoriaAgente).OrderBy(a => a.Id).ToListAsync());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CriarAgente([FromBody] AgenteRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Agente novoAgente = new Agente
        {
            Nome = request.Nome,
            CategoriaAgenteId = request.CategoriaAgenteId,
            Descricao = request.Descricao,
            Ativo = request.Ativo
        };

        _context.Agentes.Add(novoAgente);

        string payloadJson = JsonSerializer.Serialize(request);

        LogAuditoria log = new LogAuditoria
        {
            UsuarioId = GetUsuarioId(),
            Acao = $"Criou agente {novoAgente.Nome}",
            Entidade = "Agente",
            Payload = payloadJson
        };

        _context.LogsAuditoria.Add(log);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ListarAgentes), new { id = novoAgente.Id }, novoAgente);
        
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AtualizarAgente(int id, [FromBody] AgenteRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var agenteBanco = await _context.Agentes.FindAsync(id);
        if (agenteBanco == null) return NotFound();

        agenteBanco.Nome = request.Nome;
        agenteBanco.CategoriaAgenteId = request.CategoriaAgenteId;
        agenteBanco.Descricao = request.Descricao;
        agenteBanco.Ativo = request.Ativo;

        string payloadJson = JsonSerializer.Serialize(request);

        LogAuditoria log = new LogAuditoria
        {
            UsuarioId = GetUsuarioId(),
            Acao = $"Atualizou agente Id {agenteBanco.Id}",
            Entidade = "Agente",
            Payload = payloadJson
        };

        _context.LogsAuditoria.Add(log);

        await _context.SaveChangesAsync();
        return NoContent();
        
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletarAgente(int id)
    {
        var agente = await _context.Agentes.FindAsync(id);
        if (agente == null) return NotFound();

        string payloadJson = JsonSerializer.Serialize(new { id = id });

        LogAuditoria log = new LogAuditoria
        {
            UsuarioId = GetUsuarioId(),
            Acao = $"Deletou agente {agente.Nome}",
            Entidade = "Agente",
            Payload = payloadJson
        };

        try
        {
            _context.Agentes.Remove(agente);
            _context.LogsAuditoria.Add(log);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException) {
            return StatusCode(500, "Não é possível excluir o agente pois existem registros dependentes no banco.");
        }
    }

}
