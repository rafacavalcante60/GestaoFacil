using GestaoFacil.Server.Models.Domain;
using GestaoFacil.Server.Models.Principais;

namespace GestaoFacil.Server.Data
{
    //IMPORTANT: Se adicionar/remover um inicializador tem que trocar a range dos create Dto
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            var saveNeeded = false;

            if (!context.FormasPagamento.Any())
            {
                var formasPagamento = new List<FormaPagamentoModel>
                {
                    new() { Nome = "Dinheiro" },
                    new() { Nome = "Cartão de Crédito" },
                    new() { Nome = "Cartão de Débito" },
                    new() { Nome = "Pix" },
                    new() { Nome = "Cheque" },
                    new() { Nome = "Boleto" },
                    new() { Nome = "Outro" }
                };
                context.FormasPagamento.AddRange(formasPagamento);
                saveNeeded = true;
            }

            if (!context.CategoriasDespesa.Any())
            {
                context.CategoriasDespesa.AddRange(new List<CategoriaDespesaModel>
                {
                    new() { Nome = "Alimentação" },
                    new() { Nome = "Transporte" },
                    new() { Nome = "Moradia" },
                    new() { Nome = "Lazer" },
                    new() { Nome = "Educação" },
                    new() { Nome = "Saúde" },
                    new() { Nome = "Outra" }
                });
                saveNeeded = true;
            }

            if (!context.CategoriasReceita.Any())
            {
                context.CategoriasReceita.AddRange(new List<CategoriaReceitaModel>
                {
                    new() { Nome = "Salário" },
                    new() { Nome = "Presente" },
                    new() { Nome = "Venda" },
                    new() { Nome = "Investimento" },
                    new() { Nome = "Outros" }
                });
                saveNeeded = true;
            }

            if (!context.TiposUsuario.Any())
            {
                var tiposUsuario = new List<TipoUsuarioModel>
                {
                    new() { Nome = "Comum" },
                    new() { Nome = "Admin" }
                };
                context.TiposUsuario.AddRange(tiposUsuario);
                saveNeeded = true;
            }

            if (saveNeeded)
            {
                context.SaveChanges();
            }

            if (!context.Usuarios.Any(u => u.Email == "admin@gestaofacil.com"))
            {
                var tipoAdmin = context.TiposUsuario.FirstOrDefault(t => t.Nome == "Admin");
                if (tipoAdmin != null)
                {
                    var senhaEmTexto = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

                    if (string.IsNullOrEmpty(senhaEmTexto))
                    {
                        throw new Exception("Variável de ambiente ADMIN_PASSWORD não está configurada.");
                    }

                    var senhaHash = BCrypt.Net.BCrypt.HashPassword(senhaEmTexto);

                    var admin = new UsuarioModel
                    {
                        Nome = "Administrador",
                        Email = "admin@gestaofacil.com",
                        SenhaHash = senhaHash,
                        TipoUsuarioId = tipoAdmin.Id
                    };

                    context.Usuarios.Add(admin);
                    context.SaveChanges();
                }
            }

        }
    }
}
