import { Injectable } from '@angular/core';

export interface LookupItem {
  id: number;
  nome: string;
}

@Injectable({ providedIn: 'root' })
export class LookupService {
  readonly formasPagamento: LookupItem[] = [
    { id: 1, nome: 'Dinheiro' },
    { id: 2, nome: 'Cartão de Crédito' },
    { id: 3, nome: 'Cartão de Débito' },
    { id: 4, nome: 'Pix' },
    { id: 5, nome: 'Cheque' },
    { id: 6, nome: 'Boleto' },
    { id: 7, nome: 'Outro' }
  ];

  readonly categoriasDespesa: LookupItem[] = [
    { id: 1, nome: 'Alimentação' },
    { id: 2, nome: 'Transporte' },
    { id: 3, nome: 'Moradia' },
    { id: 4, nome: 'Lazer' },
    { id: 5, nome: 'Educação' },
    { id: 6, nome: 'Saúde' },
    { id: 7, nome: 'Outra' }
  ];

  readonly categoriasReceita: LookupItem[] = [
    { id: 1, nome: 'Salário' },
    { id: 2, nome: 'Presente' },
    { id: 3, nome: 'Venda' },
    { id: 4, nome: 'Investimento' },
    { id: 5, nome: 'Outros' }
  ];
}
