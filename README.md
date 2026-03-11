# GestãoFácil
<p align="center">
  <img src="https://i.imgur.com/Dp5plyS.png" alt="GestãoFácil Logo" width="180">
</p>

**GestãoFácil** é um sistema de controle financeiro desenvolvido para **microempreendedores**, como **comerciantes**, **indústrias** e **prestadores de serviços**, que buscam uma plataforma simples e eficiente para registrar **receitas** e **despesas** com clareza.

Está sendo desenvolvido com **ASP.NET Core** e **Angular**.

---

## Tecnologias usadas

Frontend:
-Framework: **Angular**

Backend:  
-API: **ASP.NET Core Web API**  
-Dados: **Entity Framework Core + MySQL**  
-Autenticação e autorização: **JWT**  
-Mapeamento de DTOs: **AutoMapper**  
-Testes unitários: **xUnit + Moq + Fluent Assertions**

## Como Rodar

### 1. Clonar o repositório

```bash
git clone https://github.com/rafacavalcante60/GestaoFacil
```

> O projeto virá sem o `appsettings.Development.json` pois contém dados sensíveis — ele deve ser adicionado manualmente (veja o passo 3 depois).

---

### 2. Pré-requisitos

Baixe e instale pelo navegador:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20 LTS](https://nodejs.org/)
- [MySQL Workbench](https://dev.mysql.com/downloads/workbench/)

---

### 3. Banco de dados (MySQL)

Abra o MySQL Workbench e crie uma nova conexão com as credenciais:  
`server=localhost; User=root; Password=@Password123;`

Renomeie o arquivo `appsettings.Example.json` para `appsettings.Development.json` dentro da pasta `GestaoFacil.Server`.

---

### 4. Variável de ambiente

```bash
setx ADMIN_PASSWORD "<3K5g£1p8z5VDYa8"
```

---

### 5. Backend

Dentro da pasta `GestaoFacil.Server`, rode no CMD:

```bash
dotnet restore
dotnet run --launch-profile "https"
```

> Caso ocorra erro de certificado, execute antes:
> ```bash
> dotnet dev-certs https --trust
> ```

---

### 6. Frontend

Dentro da pasta `GestaoFacil.Client`, rode no CMD:

```bash
npm install -g @angular/cli
npm install
ng serve
```

---

### (Opcional) Envio de e-mail

O sistema suporta envio de e-mails via SMTP (ex: [Brevo](https://www.brevo.com/)). Para ativar, preencha a seção `Email` no `appsettings.Development.json`:
```json
"Email": {
  "SmtpHost": "smtp-relay.brevo.com",
  "SmtpPort": "587",
  "SmtpUser": "SEU_USUARIO_SMTP",
  "SmtpPass": "SUA_SENHA_SMTP",
  "From": "SEU_EMAIL"
}
```

> Sem essas configurações o sistema funciona normalmente, apenas sem envio de e-mails.

---

## Telas
![Print das telas 1](https://i.imgur.com/vImgN5r.png)
![Print das telas 2](https://i.imgur.com/T6lRzbg.png)
![Print das telas 3](https://i.imgur.com/lUpdgB4.png)
![Print das telas 4](https://i.imgur.com/ulDJRcj.png)
![Print das telas 5](https://i.imgur.com/R6Jog21.png)
![Print das telas 6](https://i.imgur.com/EXyr1Af.png)

## Lista de funcionalidades

- Cadastro de usuários
- Login utilizando JWT
- Hash seguro de senhas
- Cadastro de receitas
- Cadastro de despesas
- Resumo mensal de receitas e despesas
- Cálculo de saldo
- Totais de entradas e saídas
- Indicadores financeiros básicos

---

## Documentação da API

![Print do Swagger 1](https://i.imgur.com/hmPj6CJ.png)
![Print do Swagger 2](https://i.imgur.com/kdc1yrZ.png)

## Próximas etapas

- **Serilog** (Monitorar logging)
- **Docker** (containerização do sistema)
- Deploy contínuo na **AWS**, com CI/CD via **Azure DevOps** ou **GitHub Actions**
