using GestaoFacil.Server.Models.Auth;
using GestaoFacil.Server.Models.Domain;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Models.Usuario;
using Microsoft.EntityFrameworkCore;

namespace GestaoFacil.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UsuarioModel> Usuarios { get; set; }
        public DbSet<ReceitaModel> Receitas { get; set; }
        public DbSet<DespesaModel> Despesas { get; set; }

        public DbSet<FormaPagamentoModel> FormasPagamento { get; set; }
        public DbSet<CategoriaDespesaModel> CategoriasDespesa { get; set; }
        public DbSet<CategoriaReceitaModel> CategoriasReceita { get; set; }
        public DbSet<TipoUsuarioModel> TiposUsuario { get; set; }
        public DbSet<RefreshTokenModel> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsuarioModel>()
                .HasMany(u => u.Receitas)
                .WithOne(r => r.Usuario)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsuarioModel>()
                .HasMany(u => u.Despesas)
                .WithOne(d => d.Usuario)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FormaPagamentoModel>()
                .HasMany(fp => fp.Receitas)
                .WithOne(r => r.FormaPagamento)
                .HasForeignKey(r => r.FormaPagamentoId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<FormaPagamentoModel>()
                .HasMany(fp => fp.Despesas)
                .WithOne(d => d.FormaPagamento)
                .HasForeignKey(d => d.FormaPagamentoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CategoriaDespesaModel>()
                .HasMany(cd => cd.Despesas)
                .WithOne(d => d.CategoriaDespesa)
                .HasForeignKey(d => d.CategoriaDespesaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CategoriaReceitaModel>()
                .HasMany(cr => cr.Receitas)
                .WithOne(r => r.CategoriaReceita)
                .HasForeignKey(r => r.CategoriaReceitaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TipoUsuarioModel>()
                .HasMany(tu => tu.Usuarios)
                .WithOne(u => u.TipoUsuario)
                .HasForeignKey(u => u.TipoUsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RefreshTokenModel>()
                .HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.UsuarioId);
        }
    }
}
