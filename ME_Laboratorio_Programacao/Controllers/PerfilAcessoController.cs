// Controllers/PerfilAcessoController.cs
using ME_Laboratorio_Programacao.Data;
using ME_Laboratorio_Programacao.DTOs;
using ME_Laboratorio_Programacao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ME_Laboratorio_Programacao.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Apenas administradores gerenciam perfis de acesso
public class PerfilAcessoController : ControllerBase
{
    private readonly AppDbContext _context;

    public PerfilAcessoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous] // Permite que a tela de cadastro ou filtros do front busquem os perfis
    public async Task<IActionResult> GetAll() => Ok(await _context.PerfilAcessos.ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PerfilAcessoRequest request)
    {
        var perfil = new PerfilAcesso { Nome = request.Nome, Descricao = request.Descricao };
        _context.PerfilAcessos.Add(perfil);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = perfil.Id }, perfil);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PerfilAcessoRequest request)
    {
        var perfil = await _context.PerfilAcessos.FindAsync(id);
        if (perfil == null) return NotFound();

        perfil.Nome = request.Nome;
        perfil.Descricao = request.Descricao;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var perfil = await _context.PerfilAcessos.FindAsync(id);
        if (perfil == null) return NotFound();

        _context.PerfilAcessos.Remove(perfil);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}