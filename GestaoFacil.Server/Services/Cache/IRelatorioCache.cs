namespace GestaoFacil.Server.Services.Cache
{
    // Cache dos relatorios financeiros. Esconde do resto do codigo o IDistributedCache
    // (Redis em producao, memoria do processo no fallback) e, principalmente, a forma
    // de invalidar todos os relatorios de um usuario de uma vez.
    //
    // Por que existe: os 4 relatorios varrem todas as despesas e receitas do usuario e
    // agregam em memoria. Enquanto o usuario nao grava nada, o resultado nao muda —
    // recalcular a cada F5 e desperdicio. Guardamos no Redis com TTL curto e invalidamos
    // quando ele cria/edita/remove uma despesa ou receita.
    public interface IRelatorioCache
    {
        // Le o relatorio cacheado. Retorna null em cache miss (ou se o Redis estiver fora).
        Task<T?> ObterAsync<T>(int usuarioId, string chave, CancellationToken ct = default) where T : class;

        // Grava o relatorio calculado, com o TTL configurado.
        Task GravarAsync<T>(int usuarioId, string chave, T valor, CancellationToken ct = default) where T : class;

        // Invalida de uma vez TODOS os relatorios do usuario (chamado ao gravar despesa/receita).
        Task InvalidarAsync(int usuarioId, CancellationToken ct = default);
    }
}
