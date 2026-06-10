// Controllers/UsuariosController.cs
using ME_Laboratorio_Programacao.Data;
using ME_Laboratorio_Programacao.DTOs;
using ME_Laboratorio_Programacao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ME_Laboratorio_Programacao.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Bloqueia acesso sem cookie por padrão
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuariosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<IActionResult> GetAll()
    {
        var usuarios = await _context.Usuarios
            .Include(u => u.PerfilAcesso)
            .Select(u => new { u.Id, u.Nome, u.Email, u.Ativo, Perfil = u.PerfilAcesso!.Nome })
            .ToListAsync();

        return Ok(usuarios);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // Apenas Admin cria novos usuários
    public async Task<IActionResult> Create([FromBody] UsuarioCreateRequest request)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
            return BadRequest("E-mail já cadastrado.");

        var usuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email,
            Senha = BCrypt.Net.BCrypt.HashPassword(request.Senha), // Criptografia BCrypt
            PerfilAcessoId = request.PerfilAcessoId
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = usuario.Id }, new { usuario.Id, usuario.Nome });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UsuarioUpdateRequest request)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        usuario.Nome = request.Nome;
        usuario.Email = request.Email;
        usuario.Ativo = request.Ativo;
        usuario.PerfilAcessoId = request.PerfilAcessoId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}