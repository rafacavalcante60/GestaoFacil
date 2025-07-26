using GestaoFacil.Server.Responses;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace GestaoFacil.Server.Extensions.Middleware
{
    public static class ApiExceptionMiddleware
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>(); //pega ambiente atual
                    var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>()
                                                        .CreateLogger("GlobalExceptionHandler");

                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger.LogError(contextFeature.Error, "Erro não tratado capturado no middleware.");

                        var mensagem = env.IsDevelopment()
                            ? contextFeature.Error.Message
                            : "Ocorreu um erro interno no servidor.";

                        object? dados = null;

                        if (env.IsDevelopment())
                        {
                            dados = new
                            {
                                contextFeature.Error.StackTrace
                            };
                        }

                        var response = ResponseHelper.Falha(mensagem, dados);

                        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });

                        await context.Response.WriteAsync(json);
                    }
                });
            });
        }
    }
}
