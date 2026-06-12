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
public class CategoriaAgenteController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriaAgenteController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("categorias")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ListarCategorias()
    {
        return Ok(await _context.CategoriaAgentes.ToListAsync());
    }

    [HttpPost("categorias")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CriarCategoria([FromBody] CategoriaRequest request)
    {
        var categoria = new CategoriaAgente { Nome = request.Nome, CorHex = request.CorHex };
        _context.CategoriaAgentes.Add(categoria);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(ListarCategorias), new { id = categoria.Id }, categoria);
    }

    [HttpPut("categorias/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AtualizarCategoria(int id, [FromBody] CategoriaRequest request)
    {
        var categoria = await _context.CategoriaAgentes.FindAsync(id);
        if (categoria == null) return NotFound();

        categoria.Nome = request.Nome;
        categoria.CorHex = request.CorHex;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("categorias/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletarCategoria(int id)
    {
        var categoria = await _context.CategoriaAgentes.FindAsync(id);
        if (categoria == null) return NotFound();

        _context.CategoriaAgentes.Remove(categoria);
        await _context.SaveChangesAsync();
        return NoContent();
    }

}
