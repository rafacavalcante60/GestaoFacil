# Deploy na AWS — runbook (Fase 2)

Passo a passo para subir o GestãoFácil na AWS: **EC2** (Docker: app + Caddy) + **RDS MySQL**, com HTTPS via Let's Encrypt e domínio próprio.

O código já está pronto para isso (`Dockerfile`, `docker-compose.prod.yml`, `Caddyfile`). O que falta é infraestrutura, feita no console da AWS.

Cada passo tem o **porquê** e a **armadilha** — vários dos erros abaixo custam dinheiro ou deixam o site sem HTTPS por dias.

---

## Arquitetura alvo

```
   Internet ── HTTPS :443
      ▼
┌──────────────────────┐          ┌─────────────────────┐
│  EC2 t3.micro        │   3306   │  RDS MySQL          │
│  ┌────────────────┐  │─────────▶│  db.t3.micro        │
│  │ Caddy (TLS)    │  │ (privado │  gestaofacilbanco   │
│  └───────┬────────┘  │  na VPC) └─────────────────────┘
│  ┌───────▼────────┐  │
│  │ App .NET :8080 │  │  (API /api + Angular SPA)
│  └────────────────┘  │
└──────────▲───────────┘
     Elastic IP ← registro A ← domínio
```

O TLS termina no **Caddy**; o app fala HTTP puro na rede interna do Docker e **não publica porta no host**.

---

## Antes de criar qualquer coisa

- [ ] **Budget com alerta.** Billing → Budgets → orçamento mensal de US$5, alerta em 80%.
      *Por quê:* é a única proteção contra um recurso esquecido ligado. Faça **antes**, não depois.
- [ ] **Verifique em qual free tier sua conta está.** Billing → Free tier.
      *Por quê:* a AWS mudou o modelo em meados de 2025 — contas novas recebem créditos com prazo curto em vez dos 12 meses clássicos. Isso muda a expectativa de custo por completo.
- [ ] **Escolha uma região e não saia dela.** Sugestão: `us-east-1`.
      *Por quê:* recursos em regiões diferentes não se enxergam na rede privada. RDS numa e EC2 noutra = recriar tudo. `sa-east-1` (São Paulo) tem latência melhor, mas é bem mais cara — e latência não importa em portfólio.
- [ ] **Use a VPC default.** Não crie VPC própria agora.
      *Por quê:* VPC customizada te empurra para subnet privada, que exige **NAT Gateway (~US$32/mês)** — o maior pé-na-jaca de custo de iniciante. A VPC default já vem pronta.

---

## Passo 1 — Security Groups (antes das instâncias)

Existe dependência circular: o SG do RDS aponta para o SG da EC2. Crie os dois primeiro.

- [ ] `sg-gestaofacil-ec2` — entrada: **22 só do seu IP** (opção "My IP"), **80** e **443** de `0.0.0.0/0`.
- [ ] `sg-gestaofacil-rds` — entrada: **3306 com origem = o security group `sg-gestaofacil-ec2`**, não um IP.

> **Conceito que vale aprender:** referenciar o SG em vez de um endereço faz a regra continuar válida se a EC2 trocar de IP, e garante que nenhuma outra máquina do mundo — nem dentro da AWS — alcance o banco.

---

## Passo 2 — RDS

- [ ] Create database → MySQL 8 → template **Free tier** → `db.t3.micro`, 20 GB gp3.
- [ ] **Public access = No** ← *se marcar Yes, o banco fica exposto na internet com só a senha protegendo.*
- [ ] Single-AZ (Multi-AZ dobra o custo e não serve para portfólio).
- [ ] **Desmarque Enhanced Monitoring e Performance Insights** — não são free tier e passam despercebidos na fatura.
- [ ] **Desligue o storage autoscaling** — senão cresce sozinho além dos 20 GB gratuitos.
- [ ] Security group: `sg-gestaofacil-rds`.
- [ ] Anote o **endpoint** (`algo.xxxx.us-east-1.rds.amazonaws.com`) → vai no `DB_HOST`.

Fica disponível em ~10 min. Adiante a EC2 nesse tempo.

---

## Passo 3 — EC2

- [ ] Ubuntu 24.04, **`t3.micro`**, 30 GB, security group `sg-gestaofacil-ec2`, key pair criada e baixada.

> ⚠️ **Confie no rótulo "Free tier eligible" do console**, não nesta doc: dependendo da região e da idade da conta o elegível é `t2.micro`. A escolha errada vira cobrança silenciosa.

- [ ] **Crie swap antes de buildar** — a `t3.micro` tem 1 GB de RAM e o build do Angular estoura:

```bash
sudo fallocate -l 4G /swapfile && sudo chmod 600 /swapfile
sudo mkswap /swapfile && sudo swapon /swapfile
echo '/swapfile none swap sw 0 0' | sudo tee -a /etc/fstab
```

> *Por quê:* sem swap o `docker build` morre com um `killed` que não explica nada. A linha no `/etc/fstab` é o que faz o swap sobreviver a um reboot — sem ela, a máquina reinicia e o próximo build quebra de novo.

---

## Passo 4 — Elastic IP

- [ ] Alocar **e associar imediatamente** à instância.

> **Custo real:** desde 2024 a AWS cobra por *todo* IPv4 público (~US$3,60/mês), associado ou não. Não é mais "grátis se estiver em uso" — é um custo fixo do projeto. Se um dia destruir a EC2, **libere o EIP junto**.

---

## Passo 5 — Domínio e DNS

- [ ] Registrar o domínio e criar um registro **A** apontando para o Elastic IP.

> **Mais barato:** Route 53 cobra US$0,50/mês pela hosted zone. Registrando em registro.br ou Namecheap e usando o DNS do próprio registrador, você cria só o registro A e não paga isso. Funciona igual.

- [ ] **Confirme a propagação antes de seguir:**

```bash
dig +short seu-dominio.com   # precisa devolver o seu Elastic IP
```

> ⚠️ **Não pule.** Se o Caddy tentar emitir o certificado antes do DNS propagar, o Let's Encrypt não valida o domínio — e são **5 falhas por semana** até a janela resetar, ou seja, dias sem HTTPS.

---

## Passo 6 — Subir a aplicação

```bash
# na EC2, via SSH
sudo apt update && sudo apt install -y docker.io docker-compose-v2 git
sudo usermod -aG docker $USER   # relogue depois: sem isso todo docker pede sudo

git clone git@github.com:rafacavalcante60/GestaoFacil.git
cd GestaoFacil
cp .env.prod.example .env && nano .env

docker compose -f docker-compose.prod.yml up -d --build
docker compose -f docker-compose.prod.yml logs -f
```

Preenchendo o `.env`:

- [ ] `DOMAIN`, `DB_HOST` (endpoint do RDS, **sem** a porta colada), `DB_USER`, `DB_PASSWORD`
- [ ] `JWT_KEY` — gere novo com `openssl rand -base64 48`
- [ ] `ADMIN_PASSWORD` — senha forte, nova

> 🔑 **Gere valores novos.** A chave JWT antiga e a senha do MySQL estão no histórico do git (o `appsettings.json` sempre foi commitado) — na prática são públicas. Nunca reutilize.

Nos logs você quer ver: o Caddy obtendo o certificado, e o app aplicando as migrations no RDS.

---

## Verificação final

- [ ] `https://seu-dominio` carrega o SPA, cadeado válido
- [ ] `http://seu-dominio` redireciona para HTTPS
- [ ] Login funciona (devolve JWT) e um CRUD persiste
- [ ] As tabelas existem no RDS (migrations rodaram no startup)
- [ ] Budget configurado e ativo

---

## Se der errado, olhe nesta ordem

| Sintoma | Causa quase sempre |
|---|---|
| App não conecta no banco | SG do RDS (3306 apontando para o SG errado), ou `DB_HOST` com a porta colada no endpoint |
| Sem HTTPS / erro de certificado | DNS não propagado quando o Caddy tentou emitir |
| `docker build` morre sem mensagem | Falta de RAM — o swap do Passo 3 |
| Todo `docker` pede sudo | Faltou relogar após o `usermod -aG docker` |

Logs: `docker compose -f docker-compose.prod.yml logs -f app` (ou `caddy`).

---

## Custo esperado

Com free tier de 12 meses: EC2 + RDS + EBS ≈ **US$0**, mais **IPv4 ~US$3,60/mês** e domínio ~US$10–15/ano (+ US$0,50/mês se usar hosted zone do Route 53).

Depois dos 12 meses: EC2 ~US$7–8 + RDS ~US$12–15 ≈ **US$20–25/mês**. Para baratear: desligar EC2/RDS quando não estiver mostrando o projeto, ou migrar o MySQL para container na própria EC2.
