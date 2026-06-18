// Controllers/UsuariosController.cs
using ME_Laboratorio_Programacao.Data;
using ME_Laboratorio_Programacao.DTOs;
using ME_Laboratorio_Programacao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ME_Laboratorio_Programacao.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuariosController(AppDbContext context)
    {
        _context = context;
    }

    private int GetUsuarioId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<IActionResult> GetAll()
    {
        var usuarios = await _context.Usuarios
            .Include(u => u.PerfilAcesso)
            .Select(u => new { u.Id, u.Nome, u.Email, u.Ativo, Perfil = u.PerfilAcesso!.Nome, u.PerfilAcessoId })
            .ToListAsync();

        return Ok(usuarios);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] UsuarioCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
            return BadRequest("E-mail já cadastrado.");

        var usuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email,
            Senha = GerarHashMd5(request.Senha),
            PerfilAcessoId = request.PerfilAcessoId
        };

        _context.Usuarios.Add(usuario);

        string payloadJson = JsonSerializer.Serialize(new { request.Nome, request.Email, Senha = "*******", request.PerfilAcessoId });

        LogAuditoria log = new LogAuditoria
        {
            UsuarioId = GetUsuarioId(),
            Acao = $"Criou usuário {usuario.Nome}",
            Entidade = "Usuario",
            Payload = payloadJson
        };

        _context.LogsAuditoria.Add(log);

        await _context.SaveChangesAsync();

        return Ok(usuario);

    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UsuarioUpdateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        usuario.Nome = request.Nome;
        usuario.Ativo = request.Ativo;
        usuario.PerfilAcessoId = request.PerfilAcessoId;

        if (!string.IsNullOrWhiteSpace(request.Senha))
        {
            usuario.Senha = GerarHashMd5(request.Senha);
        }

        string payloadJson = JsonSerializer.Serialize(new { request.Nome, Senha = "*******", request.Ativo, request.PerfilAcessoId});

        LogAuditoria log = new LogAuditoria
        {
            UsuarioId = GetUsuarioId(),
            Acao = $"Alterou usuário Id {usuario.Id}",
            Entidade = "Usuario",
            Payload = payloadJson
        };

        _context.LogsAuditoria.Add(log);

        await _context.SaveChangesAsync();
        return Ok("Usuário atualizado");

    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        string payloadJson = JsonSerializer.Serialize(new { id = id });

        LogAuditoria log = new LogAuditoria
        {
            UsuarioId = GetUsuarioId(),
            Acao = $"Deletou usuário {usuario.Nome}",
            Entidade = "Usuario",
            Payload = payloadJson
        };

        try
        {
            _context.Usuarios.Remove(usuario);
            _context.LogsAuditoria.Add(log);
            await _context.SaveChangesAsync();
            return Ok("Usuário deletado");
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            return StatusCode(500, "Não é possível excluir o usuário pois existem agentes vinculados a ele no banco de dados.");
        }
    }

    private string GerarHashMd5(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}