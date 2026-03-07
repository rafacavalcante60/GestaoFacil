export interface Meta {
  id: number;
  nome: string;
  valorMeta: number;
  tipo: 1 | 2; // 1 = Despesa, 2 = Receita
  categoriaDespesaId?: number;
  categoriaReceitaId?: number;
  dataInicio: string;
  dataFim: string;
  ativo: boolean;
  // calculados pelo backend
  valorAtual: number;
  percentual: number;
  statusMeta: string;
}
