using Prova_Laboratorio_Programacao.Data;
using Prova_Laboratorio_Programacao.DTOs;
using Prova_Laboratorio_Programacao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Prova_Laboratorio_Programacao.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class PerfilAcessoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PerfilAcessoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll() => Ok(await _context.PerfilAcessos.ToListAsync());
    }
}
