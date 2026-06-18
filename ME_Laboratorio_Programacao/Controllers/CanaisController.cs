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
public class CanaisController : ControllerBase
{
    private readonly AppDbContext _context;

    public CanaisController(AppDbContext context)
    {
        _context = context;
    }
    private int GetUsuarioId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<IActionResult> ListarCanais()
    {
        return Ok(await _context.CanaisOrigem.ToListAsync());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CriarCanal([FromBody] CanalRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var canal = new CanalOrigem { Nome = request.Nome, Ativo = request.Ativo };

        string payloadJson = JsonSerializer.Serialize(request);

        LogAuditoria log = new LogAuditoria
        {
            UsuarioId = GetUsuarioId(),
            Acao = $"Criou canal de origem {canal.Nome}",
            Entidade = "CanalOrigem",
            Payload = payloadJson
        };

        _context.LogsAuditoria.Add(log);

        _context.CanaisOrigem.Add(canal);
        await _context.SaveChangesAsync();
        return Ok(canal);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AtualizarCanal(int id, [FromBody] CanalRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var canal = await _context.CanaisOrigem.FindAsync(id);
        if (canal == null) return NotFound();

        canal.Nome = request.Nome;
        canal.Ativo = request.Ativo;

        string payloadJson = JsonSerializer.Serialize(request);

        LogAuditoria log = new LogAuditoria
        {
            UsuarioId = GetUsuarioId(),
            Acao = $"Atualizou canal de origem (ID: {id})",
            Entidade = "CanalOrigem",
            Payload = payloadJson
        };

        _context.LogsAuditoria.Add(log);

        await _context.SaveChangesAsync();
        return Ok("Canal atualizado com sucesso!");
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletarCanal(int id)
    {
        var canal = await _context.CanaisOrigem.FindAsync(id);
        if (canal == null) return NotFound();

        string payloadJson = JsonSerializer.Serialize(new { id = id });

        LogAuditoria log = new LogAuditoria
        {
            UsuarioId = GetUsuarioId(),
            Acao = $"Deletou canal de origem {canal.Nome}",
            Entidade = "CanalOrigem",
            Payload = payloadJson
        };

        try
        {
            _context.CanaisOrigem.Remove(canal);
            _context.LogsAuditoria.Add(log);
            await _context.SaveChangesAsync();
            return Ok("Canal deletado com sucesso!");
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException) {
            return StatusCode(500, "Não é possível excluir o canal pois existem registros dependentes no banco.");
        }
    }

}
