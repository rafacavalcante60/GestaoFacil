//Extensions com os codigos
using GestaoFacil.Server.Extensions.Middleware;
using GestaoFacil.Server.Extensions.Service;

var builder = WebApplication.CreateBuilder(args);

// serviços principais
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

// CORS
builder.Services.AddCorsPolicy("AllowAngular", "http://localhost:4200");

// logger
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// inicializa o banco de dados
app.InitializeDatabase();

// middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
    app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("Index.html");

app.Run();
