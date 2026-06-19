using Prova_Laboratorio_Programacao.Data;
using Prova_Laboratorio_Programacao.DTOs;
using Prova_Laboratorio_Programacao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace Prova_Laboratorio_Programacao.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class CategoriaAgenteController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriaAgenteController(AppDbContext context)
    {
        _context = context;
    }

    private int GetUsuarioId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ListarCategorias()
    {
        return Ok(await _context.CategoriaAgentes.ToListAsync());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CriarCategoria([FromBody] CategoriaRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var categoria = new CategoriaAgente { Nome = request.Nome, CorHex = request.CorHex };
        _context.CategoriaAgentes.Add(categoria);

        string payloadJson = JsonSerializer.Serialize(request);

        LogAuditoria log = new LogAuditoria
        {
            UsuarioId = GetUsuarioId(),
            Acao = $"Criou categoria {categoria.Nome}",
            Entidade = "CategoriaAgente",
            Payload = payloadJson
        };

        _context.LogsAuditoria.Add(log);

        await _context.SaveChangesAsync();
        return Ok(categoria);

    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AtualizarCategoria(int id, [FromBody] CategoriaRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var categoria = await _context.CategoriaAgentes.FindAsync(id);
        if (categoria == null) return NotFound();

        categoria.Nome = request.Nome;
        categoria.CorHex = request.CorHex;

        string payloadJson = JsonSerializer.Serialize(request);

        LogAuditoria log = new LogAuditoria
        {
            UsuarioId = GetUsuarioId(),
            Acao = $"Alterou categoria Id {categoria.Id}",
            Entidade = "CategoriaAgente",
            Payload = payloadJson
        };

        _context.LogsAuditoria.Add(log);

        await _context.SaveChangesAsync();
        return Ok("Categoria atualizada com sucesso!");

    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletarCategoria(int id)
    {
        var categoria = await _context.CategoriaAgentes.FindAsync(id);
        if (categoria == null) return NotFound();

        string payloadJson = JsonSerializer.Serialize(new { id = id });

        LogAuditoria log = new LogAuditoria
        {
            UsuarioId = GetUsuarioId(),
            Acao = $"Deletou categoria {categoria.Nome}",
            Entidade = "CategoriaAgente",
            Payload = payloadJson
        };

        try
        {
            _context.CategoriaAgentes.Remove(categoria);
            _context.LogsAuditoria.Add(log);
            await _context.SaveChangesAsync();
            return Ok("Categoria deletada com sucesso!");
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException) {
            return StatusCode(500, "Não é possível excluir a categoria pois existem agentes vinculados a ela no banco de dados.");
        }
    }

}
