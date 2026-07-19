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
    // o proxy e o Caddy no mesmo host, em rede docker: nao ha lista fixa de IPs
    // para confiar, entao limpamos as redes/proxies conhecidos. So e seguro porque
    // a porta 8080 do app nunca fica exposta fora da rede do compose.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
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
