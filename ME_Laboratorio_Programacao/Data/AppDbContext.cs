using ME_Laboratorio_Programacao.Models;
using ME_Laboratorio_Programacao.Models.Mensagens;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ME_Laboratorio_Programacao.Data;

public class AppDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AppDbContext(DbContextOptions<AppDbContext> options,
        IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<PerfilAcesso> PerfilAcessos => Set<PerfilAcesso>();
    public DbSet<CategoriaAgente> CategoriaAgentes => Set<CategoriaAgente>();
    public DbSet<Agente> Agentes => Set<Agente>();
    public DbSet<CanalOrigem> CanaisOrigem => Set<CanalOrigem>();
    public DbSet<SessaoAtendimento> SessoesAtendimento => Set<SessaoAtendimento>();
    public DbSet<Mensagem> Mensagens => Set<Mensagem>();
    public DbSet<ContextoMemoria> ContextosMemoria => Set<ContextoMemoria>();
    public DbSet<LogAuditoria> LogsAuditoria => Set<LogAuditoria>();
    public DbSet<EstatisticaAcesso> EstatisticasAcesso => Set<EstatisticaAcesso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PerfilAcesso>(entity =>
        {
            entity.ToTable("PerfilAcesso");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasOne(d => d.PerfilAcesso)
                  .WithMany(p => p.Usuarios)
                  .HasForeignKey(d => d.PerfilAcessoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Agente>()
            .ToTable("Agente");

        modelBuilder.Entity<Agente>()
            .HasOne(a => a.CategoriaAgente)
            .WithMany(c => c.Agentes)
            .HasForeignKey(a => a.CategoriaAgenteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Mensagem>()
            .HasOne(m => m.SessaoAtendimento)
            .WithMany(s => s.Mensagens)
            .HasForeignKey(m => m.SessaoAtendimentoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LogAuditoria>()
            .ToTable("LogAuditoria");

        modelBuilder.Entity<LogAuditoria>()
            .HasOne(l => l.Usuario)
            .WithMany()
            .HasForeignKey(l => l.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<CategoriaAgente>()
            .ToTable("CategoriaAgente");

        modelBuilder.Entity<CanalOrigem>()
            .ToTable("CanalOrigem");
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ProcessarAuditoria();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ProcessarAuditoria()
    {
        ChangeTracker.DetectChanges();

        var httpContext = _httpContextAccessor.HttpContext;
        int? usuarioIdLogado = null;

        var idClaim = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(idClaim, out int id))
        {
            usuarioIdLogado = id;
        }

        foreach (var entry in ChangeTracker.Entries().ToList())
        {
            // Evita loop infinito ignorando alterações na própria tabela de Log
            if (entry.Entity is LogAuditoria || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            // Filtro para monitorar apenas a tabela de Usuários
            //if (entry.Entity is not Usuario)
            //    continue;

            string acao = entry.State switch
            {
                EntityState.Added => "Cadastrar",
                EntityState.Modified => "Editar",
                EntityState.Deleted => "Deletar",
                _ => "Desconhecida"
            };

            var nomeEntidade = entry.Entity.GetType().Name;
            if (nomeEntidade.Contains("Proxy"))
                nomeEntidade = entry.Entity.GetType().BaseType?.Name ?? nomeEntidade;

            // --- CRIA O LOG ---
            var log = new LogAuditoria
            {
                UsuarioId = usuarioIdLogado,
                Acao = $"{acao} {nomeEntidade}",
                Entidade = nomeEntidade
            };

            LogsAuditoria.Add(log);
        }
    }
}