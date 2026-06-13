using ME_Laboratorio_Programacao.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ME_Laboratorio_Programacao.DTOs;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ME_Laboratorio_Programacao.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.PerfilAcesso)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        var hashSenha = GerarHashMd5(request.Senha);

        if (usuario == null || !usuario.Ativo || usuario.Senha != hashSenha)
        {
            return Unauthorized("E-mail ou senha inválidos.");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.PerfilAcesso?.Nome ?? "Visualizador")
        };

        var identity = new ClaimsIdentity(claims, "CookieAuth");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("CookieAuth", principal, new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
        });

        return Ok(new { mensagem = "Login realizado com sucesso", usuario = usuario.Nome, perfil = usuario.PerfilAcesso?.Nome });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return Ok(new { mensagem = "Logout realizado com sucesso" });
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