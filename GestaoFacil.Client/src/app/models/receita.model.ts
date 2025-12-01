export interface Receita {
  id?: number;
  nome: string;
  data: string; // ISO 8601 (ex: "2025-11-25T23:40:17.314Z")
  descricao?: string;
  valor: number;
  categoriaReceitaId?: number;
  formaPagamentoId?: number;
}
