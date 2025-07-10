using GestaoFacil.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GestaoFacil.Server.Services;
using GestaoFacil.Server.Mappings;
using GestaoFacil.Server.Repositories;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args); //BLOCO do container de injecao de dependencia

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GestaoFacil", Version = "v1" });

    // Adiciona suporte ao JWT Bearer
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
            new string[] {}
        }
    });
});


//Config Banco
var connectionString = builder.Configuration.GetConnectionString("AppDbConnectionString");
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// injecao de services
builder.Services.AddScoped<IDespesaService, DespesaService>();
builder.Services.AddScoped<IReceitaService, ReceitaService>();

//injecao de repositories
builder.Services.AddScoped<IDespesaRepository, DespesaRepository>();
builder.Services.AddScoped<IReceitaRepository, ReceitaRepository>();

//Config JWT
var key = Encoding.ASCII.GetBytes("2f5a8e3d7c1b9a4f6e8d2c5a7b3e9f1a2d4e6f8c3b5a7d9e1f2a3b4c5d6e7f8a9b0");

builder.Services.AddAuthentication(options =>
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
builder.Services.AddAuthorization();
builder.Services.AddScoped<TokenService>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

//Ilogger
builder.Logging.ClearProviders(); // Limpa provedores padrão (opcional)
builder.Logging.AddConsole(); // Adiciona logs no console
builder.Logging.AddDebug(); // Adiciona logs no Debug do Visual Studio

var app = builder.Build(); //BLOCO dos middlewares

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("Index.html");

app.Run();
