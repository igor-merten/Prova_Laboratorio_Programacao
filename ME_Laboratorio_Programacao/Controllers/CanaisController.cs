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
public class CanaisController : ControllerBase
{
    private readonly AppDbContext _context;

    public CanaisController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ListarCanais()
    {
        return Ok(await _context.CanaisOrigem.ToListAsync());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CriarCanal([FromBody] CanalRequest request)
    {
        var canal = new CanalOrigem { Nome = request.Nome, Ativo = request.Ativo };
        _context.CanaisOrigem.Add(canal);
        await _context.SaveChangesAsync();
        return Ok(canal);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AtualizarCanal(int id, [FromBody] CanalRequest request)
    {
        var canal = await _context.CanaisOrigem.FindAsync(id);
        if (canal == null) return NotFound();

        canal.Nome = request.Nome;
        canal.Ativo = request.Ativo;

        await _context.SaveChangesAsync();
        return Ok("Canal atualizado com sucesso!");
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletarCanal(int id)
    {
        var canal = await _context.CanaisOrigem.FindAsync(id);
        if (canal == null) return NotFound();

        _context.CanaisOrigem.Remove(canal);
        await _context.SaveChangesAsync();
        return Ok("Canal deletado com sucesso!");
    }

}
