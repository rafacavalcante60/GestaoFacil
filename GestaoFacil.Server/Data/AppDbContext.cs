using Microsoft.EntityFrameworkCore;
using GestaoFacil.Server.Models;
using GestaoFacil.Shared.Dtos;

namespace GestaoFacil.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //tabelas
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Receita> Receitas { get; set; }
        public DbSet<Despesa> Despesas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //converter enums para string no bd
            modelBuilder.Entity<Receita>()
                .Property(r => r.FormaPagamento)
                .HasConversion<string>(); 

            modelBuilder.Entity<Receita>()
                .Property(r => r.Categoria)
                .HasConversion<string>();

            modelBuilder.Entity<Despesa>()
                .Property(d => d.FormaPagamento)
                .HasConversion<string>(); 

            modelBuilder.Entity<Receita>()
                .Property(r => r.Categoria)
                .HasConversion<string>();

            modelBuilder.Entity<Usuario>()
                .Property(u => u.TipoUsuario)
                .HasConversion<string>();

            //relacionamento usuario -> receita e usuario -> despesa
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Receitas)
                .WithOne(r => r.Usuario)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade); //usuario excluido = receitas vinculadas excluidas

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Despesas)
                .WithOne(d => d.Usuario)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade); //usuario excluido = despesas vinculadas excluidas
        }
    }
}