# GestãoFácil

O **GestãoFácil** é um sistema de controle financeiro desenvolvido especialmente para **microempreendedores**, sejam **comerciantes**, **indústrias** ou **prestadores de serviço**. A proposta é oferecer uma **plataforma simples, intuitiva e eficiente** para gerenciar **receitas** e **despesas**, com funcionalidades que ajudam o usuário a **visualizar e filtrar seus lançamentos financeiros** com facilidade.

---

## Projeto em Desenvolvimento

Este projeto está em desenvolvimento utilizando **ASP.NET Core com Blazor WebAssembly**, seguindo uma arquitetura moderna e escalável, com separação clara de responsabilidades:

- **Autenticação JWT**
- **DTOs para transporte seguro de dados**
- **Controllers organizados por responsabilidade**
- **Camada de Service para regras de negócio**
- **Repository Pattern para acesso a dados**
- **APIs RESTful**

---

## Funcionalidades Planejadas

- Registro de **receitas** e **despesas**
- Relatórios financeiros simples filtrados por **data**, **tipo**, **forma de pagamento**
- Autenticação e autorização com **JWT**
- Suporte a múltiplos usuários 

---

## Tecnologias Utilizadas

- `ASP.NET Core` (Web API)
- `Blazor WebAssembly` (frontend)
- `Entity Framework Core` (ORM)
- `MySQL` (Banco de dados relacional)
- `JWT` (Autenticação baseada em token)
- `AutoMapper` (Mapeamento de entidades e DTOs)
- `FluentValidation` (validações avançadas)

Importante:
O arquivo appsettings.json é ignorado pelo Git pois contém configurações locais sensíveis.
Está disponível no repositório o arquivo appsettings.example.json, que deve ser copiado para appsettings.json e configurado antes de rodar o projeto.
