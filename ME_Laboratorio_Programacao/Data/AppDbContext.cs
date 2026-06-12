using ME_Laboratorio_Programacao.Models;
using ME_Laboratorio_Programacao.Models.Agentes;
using ME_Laboratorio_Programacao.Models.Mensagens;
using Microsoft.EntityFrameworkCore;

namespace ME_Laboratorio_Programacao.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<PerfilAcesso> PerfilAcessos => Set<PerfilAcesso>();
    public DbSet<CategoriaAgente> CategoriaAgentes => Set<CategoriaAgente>();
    public DbSet<AgenteBase> Agentes => Set<AgenteBase>(); 
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

        modelBuilder.Entity<CategoriaAgente>()
        .ToTable("CategoriaAgente");

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

        modelBuilder.Entity<AgenteBase>()
            .ToTable("Agente")
            .HasDiscriminator<string>("TipoAgente")
            .HasValue<AgentePadrao>("Padrao")
            .HasValue<SuperAgente>("Super");

        modelBuilder.Entity<AgenteBase>()
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
            .HasOne(l => l.Usuario)
            .WithMany()
            .HasForeignKey(l => l.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

    }
}
