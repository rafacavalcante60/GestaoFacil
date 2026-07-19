using GestaoFacil.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoFacil.Server.Extensions.Service
{
    public static class DatabaseServiceExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AppDbConnectionString")
                ?? throw new InvalidOperationException(
                    "Connection string ausente. Defina ConnectionStrings__AppDbConnectionString como variavel de ambiente.");
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            return services;
        }

        public static void InitializeDatabase(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate(); //cria o banco e aplica migrations automaticamente
            DbInitializer.Seed(context);
        }
    }
}
