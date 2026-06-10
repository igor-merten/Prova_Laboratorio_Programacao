using ME_Laboratorio_Programacao.Models;
using Microsoft.EntityFrameworkCore;

namespace ME_Laboratorio_Programacao.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<PerfilAcesso> PerfilAcessos => Set<PerfilAcesso>();

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
    }
}
