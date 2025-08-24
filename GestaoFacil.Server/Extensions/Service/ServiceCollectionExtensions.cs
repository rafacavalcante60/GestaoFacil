using GestaoFacil.Server.Mappings;
using GestaoFacil.Server.Repositories.Despesa;
using GestaoFacil.Server.Repositories.Financeiro;
using GestaoFacil.Server.Repositories.Usuario;
using GestaoFacil.Server.Services.Auth;
using GestaoFacil.Server.Services.Despesa;
using GestaoFacil.Server.Services.Email;
using GestaoFacil.Server.Services.Financeiro;
using GestaoFacil.Server.Services.Relatorio;
using GestaoFacil.Server.Services.Usuario;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

namespace GestaoFacil.Server.Extensions.Service
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IDespesaService, DespesaService>();
            services.AddScoped<IReceitaService, ReceitaService>();
            services.AddScoped<IRelatorioService, RelatorioService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();

            // Wrapper do SMTP
            services.AddScoped<ISmtpClientWrapper>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                return new SmtpClientWrapper(
                    config["Email:SmtpHost"],
                    int.Parse(config["Email:SmtpPort"]),
                    config["Email:SmtpUser"],
                    config["Email:SmtpPass"]);
            });

            // EmailService usando o wrapper
            services.AddScoped<IEmailService>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                Func<ISmtpClientWrapper> factory = () => sp.GetRequiredService<ISmtpClientWrapper>();
                return new EmailService(config, factory);
            });

            return services;
        }

        public static IServiceCollection AddCustomRepositories(this IServiceCollection services)
        {
            services.AddScoped<IDespesaRepository, DespesaRepository>();
            services.AddScoped<IReceitaRepository, ReceitaRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            return services;
        }

        public static IServiceCollection AddCustomAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));
            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GestaoFacil", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o token JWT no formato: Bearer {seu_token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
            });

            return services;
        }



        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, string policyName, string origin)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(policyName, policy =>
                {
                    policy.WithOrigins(origin)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            return services;
        }

        public static IServiceCollection AddRateLimitingPolicies(this IServiceCollection services, IConfiguration configuration)
            {
                services.AddRateLimiter(options =>
                {
                    //Global
                    var globalPermitLimit = configuration.GetValue<int>("RateLimiting:Global:PermitLimit", 100);
                    var globalWindowMinutes = configuration.GetValue<int>("RateLimiting:Global:WindowMinutes", 1);
                    var globalQueueLimit = configuration.GetValue<int>("RateLimiting:Global:QueueLimit", 0);

                    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    {
                        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                        return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = globalPermitLimit,
                            Window = TimeSpan.FromMinutes(globalWindowMinutes),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = globalQueueLimit
                        });
                    });

                    //Pra login
                    var loginPermitLimit = configuration.GetValue<int>("RateLimiting:Login:PermitLimit", 3);
                    var loginWindowSeconds = configuration.GetValue<int>("RateLimiting:Login:WindowSeconds", 30);
                    var loginQueueLimit = configuration.GetValue<int>("RateLimiting:Login:QueueLimit", 0);

                    options.AddFixedWindowLimiter("login", opt =>
                    {
                        opt.PermitLimit = loginPermitLimit;
                        opt.Window = TimeSpan.FromSeconds(loginWindowSeconds);
                        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                        opt.QueueLimit = loginQueueLimit;
                    });

                    options.OnRejected = async (context, token) =>
                    {
                        context.HttpContext.Response.StatusCode = 429;
                        await context.HttpContext.Response.WriteAsync(
                            "Muitas requisições. Tente novamente mais tarde.",
                            cancellationToken: token);
                    };
                });

                return services;
            }
        }
    }
