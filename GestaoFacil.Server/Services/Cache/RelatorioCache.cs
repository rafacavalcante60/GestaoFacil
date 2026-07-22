using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace GestaoFacil.Server.Services.Cache
{
    public class RelatorioCache : IRelatorioCache
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RelatorioCache> _logger;
        private readonly TimeSpan _ttl;

        // Reutiliza uma unica instancia: JsonSerializerOptions e caro de criar e e thread-safe.
        private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

        // A "versao" do usuario dura muito mais que os dados: ela e o ponteiro que diz
        // qual geracao de cache esta valida. Nao pode expirar antes dos dados, senao um
        // miss de versao geraria uma versao nova e descartaria cache ainda quente.
        private static readonly TimeSpan TtlVersao = TimeSpan.FromDays(30);

        public RelatorioCache(IDistributedCache cache, IConfiguration config, ILogger<RelatorioCache> logger)
        {
            _cache = cache;
            _logger = logger;
            var minutos = config.GetValue("RelatorioCache:TtlMinutos", 5);
            _ttl = TimeSpan.FromMinutes(minutos);
        }

        public async Task<T?> ObterAsync<T>(int usuarioId, string chave, CancellationToken ct = default) where T : class
        {
            try
            {
                var versao = await ObterVersaoAsync(usuarioId, ct);
                var json = await _cache.GetStringAsync(ChaveDados(usuarioId, versao, chave), ct);
                return json is null ? null : JsonSerializer.Deserialize<T>(json, JsonOpts);
            }
            catch (Exception ex)
            {
                // Cache e otimizacao, nao fonte da verdade. Se o Redis cair, o relatorio
                // ainda tem que ser calculado do banco: logamos e tratamos como miss.
                _logger.LogWarning(ex, "Falha ao ler cache de relatorio do usuario {UsuarioId}", usuarioId);
                return null;
            }
        }

        public async Task GravarAsync<T>(int usuarioId, string chave, T valor, CancellationToken ct = default) where T : class
        {
            try
            {
                var versao = await ObterVersaoAsync(usuarioId, ct);
                var json = JsonSerializer.Serialize(valor, JsonOpts);
                await _cache.SetStringAsync(
                    ChaveDados(usuarioId, versao, chave),
                    json,
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _ttl },
                    ct);
            }
            catch (Exception ex)
            {
                // Se nao deu para gravar, o proximo request so recalcula. Nao propaga o erro.
                _logger.LogWarning(ex, "Falha ao gravar cache de relatorio do usuario {UsuarioId}", usuarioId);
            }
        }

        public async Task InvalidarAsync(int usuarioId, CancellationToken ct = default)
        {
            try
            {
                // Nao apagamos chave por chave — o IDistributedCache nem permite varrer chaves,
                // e no Redis um SCAN + DEL seria O(n). Em vez disso trocamos a versao do usuario:
                // todas as chaves de dados antigas passam a apontar para uma versao que ninguem
                // mais le e somem sozinhas pelo TTL. Invalidacao O(1), sem tocar nas chaves velhas.
                await _cache.SetStringAsync(
                    ChaveVersao(usuarioId),
                    Guid.NewGuid().ToString("N"),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TtlVersao },
                    ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao invalidar cache de relatorio do usuario {UsuarioId}", usuarioId);
            }
        }

        // Le a versao atual do usuario; se ainda nao existe, cria uma. Uma corrida aqui
        // (dois leitores criando versoes diferentes) so custa um miss extra, nunca dado errado.
        private async Task<string> ObterVersaoAsync(int usuarioId, CancellationToken ct)
        {
            var versao = await _cache.GetStringAsync(ChaveVersao(usuarioId), ct);
            if (versao is not null) return versao;

            versao = Guid.NewGuid().ToString("N");
            await _cache.SetStringAsync(
                ChaveVersao(usuarioId),
                versao,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TtlVersao },
                ct);
            return versao;
        }

        private static string ChaveVersao(int usuarioId) => $"relatorio:versao:{usuarioId}";

        private static string ChaveDados(int usuarioId, string versao, string chave) =>
            $"relatorio:{usuarioId}:{versao}:{chave}";
    }
}
