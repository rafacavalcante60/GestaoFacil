//Extensions com os codigos
using GestaoFacil.Server.Extensions.Middleware;
using GestaoFacil.Server.Extensions.Service;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// servi�os principais
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDatabase(builder.Configuration);

// injecoes
builder.Services.AddCustomServices();
builder.Services.AddRelatorioCache(builder.Configuration);
builder.Services.AddCustomRepositories();
builder.Services.AddCustomAutoMapper();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwt();

//Rate limiting
builder.Services.AddRateLimitingPolicies(builder.Configuration);

// CORS: so no Development. Em producao o Angular e servido pela propria API (mesma origem).
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCorsPolicy("AllowAngular", "http://localhost:4200");
}

// logger
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Atras do Caddy o app so enxerga o IP do proxy e o esquema http. Sem isso o rate
// limiting (que particiona por RemoteIpAddress) joga todo mundo no mesmo balde.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();

    // Confiamos apenas na faixa privada da rede do compose (o Caddy), nao em
    // qualquer remetente: X-Forwarded-For vem do cliente e e trivial de forjar.
    // qualificado: System.Net tambem tem um IPNetwork (.NET 8) e o KnownNetworks
    // espera o do HttpOverrides — sem isto o compilador acusa ambiguidade.
    options.KnownNetworks.Add(new IPNetwork(System.Net.IPAddress.Parse("172.16.0.0"), 12));
    options.KnownNetworks.Add(new IPNetwork(System.Net.IPAddress.Parse("192.168.0.0"), 16));
    options.KnownNetworks.Add(new IPNetwork(System.Net.IPAddress.Parse("10.0.0.0"), 8));

    // O ponto central: le so o valor mais a direita da cadeia — o que o Caddy
    // escreveu — e descarta o que o cliente tenha injetado antes. Sem este limite,
    // dava para trocar de IP a cada requisicao e zerar o rate limit de login,
    // liberando forca bruta de senha sem freio.
    options.ForwardLimit = 1;
});

var app = builder.Build();

// inicializa o banco de dados
app.InitializeDatabase();

// middlewares
// precisa vir antes de tudo que le IP ou esquema da requisicao
app.UseForwardedHeaders();

// o handler ja diferencia Development (stack trace) de producao (mensagem generica),
// e em producao e justamente onde precisamos dele: sem ele um erro nao tratado vira
// um 500 de corpo vazio, fora do contrato JSON do resto da API.
app.ConfigureExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// em producao o TLS termina no Caddy: o app fala HTTP puro na rede interna e
// redirecionar aqui geraria loop. Quem forca HTTPS e o Caddy.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAngular");
}

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapRazorPages();
app.MapControllers();
// minusculo: o arquivo gerado pelo Angular e "index.html" e o filesystem do
// container Linux e case-sensitive (no Windows "Index.html" funcionava por acaso).
app.MapFallbackToFile("index.html");

app.Run();
