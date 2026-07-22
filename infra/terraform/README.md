# Infra as Code — GestaoFacil (Terraform + Azure)

Toda a infra da GestaoFacil descrita como código: uma VM Linux na Azure com
firewall (NSG), rede privada, IP público estático e FQDN gratuito. Roda a stack
de containers (MySQL + app .NET + Caddy) definida nos `docker-compose` da raiz.

> **Por que isto existe:** a infra foi criada à mão no portal/`az` durante a
> Fase 2. Isso funciona, mas não é reproduzível, versionável nem revisável.
> Com Terraform, `terraform apply` recria tudo do zero, `git log` registra cada
> mudança de firewall/tamanho de VM, e um PR permite revisar antes de aplicar.

## O que é criado

| Recurso Terraform | O que é | Correspondente no portal |
|---|---|---|
| `azurerm_resource_group` | "pasta" de tudo | Resource group `rg-gestaofacil` |
| `azurerm_virtual_network` + `azurerm_subnet` | LAN privada da VM | VNet `gestaofacil-vnet` |
| `azurerm_network_security_group` | firewall (22 só do seu IP; 80/443 públicos) | NSG `gestaofacil-nsg` |
| `azurerm_public_ip` | IP estático + FQDN grátis | `gestaofacil-ip` |
| `azurerm_network_interface` (+ associação ao NSG) | placa de rede | NIC da VM |
| `azurerm_linux_virtual_machine` | a VM Ubuntu 22.04 | VM `gestaofacil` |

Portas internas (8080 do app, 3306 do MySQL, 6379 do Redis) **não** entram no
NSG de propósito — nunca são expostas; só o Caddy fala com o mundo.

## Pré-requisitos

- [Terraform](https://developer.hashicorp.com/terraform/install) >= 1.5
- Azure CLI logada: `az login` (o Terraform reaproveita essa sessão)
- Confirmar a subscription: `az account show`

## Uso (recriar do zero — o caminho seguro)

```bash
cd infra/terraform
cp terraform.tfvars.example terraform.tfvars   # ajuste meu_ip_ssh e a chave pública

terraform init      # baixa o provider azurerm
terraform fmt       # formata os .tf (padrão do time)
terraform validate  # valida a sintaxe sem tocar na Azure
terraform plan       # mostra o que SERIA criado (nenhuma mudança ainda)
terraform apply     # cria de verdade (pede confirmação)
```

No fim, os `outputs` imprimem o IP, o FQDN e o comando de SSH. Depois é só o
provisionamento da VM (Docker, clonar o repo, `.env`, `docker compose up`) —
esse passo **não** está no Terraform (ver "Escopo", abaixo).

Para destruir tudo que o Terraform criou: `terraform destroy`.

## ⚠️ Este código NÃO gerencia a VM que está no ar

Os arquivos aqui **descrevem** a infra atual, mas o Terraform ainda **não a
controla** — não existe `.tfstate` apontando pra ela. Rodar `apply` com os
defaults tentaria **criar uma segunda** infra e falharia por nomes já em uso
(`rg-gestaofacil` já existe). Isso é proposital: proteção contra derrubar o site
por engano. Duas formas de usar:

- **Recriar do zero** (portfólio, seguro): rode com um `prefixo`/`dns_label`
  diferentes (ver `terraform.tfvars.example`) numa RG nova. Prova que o código
  levanta tudo sozinho, sem encostar na produção.

- **Adotar a VM viva** (`terraform import`, avançado): traz a infra existente
  pro controle do Terraform. Poderoso, mas **arriscado** — se algum atributo do
  código divergir do real, o `apply` seguinte pode propor **recriar a VM** e
  derrubar o site. Faça um a um e confira que o `plan` fica **"0 to add, 0 to
  destroy"** antes de qualquer `apply`:

  ```bash
  SUB=$(az account show --query id -o tsv)
  terraform import azurerm_resource_group.rg /subscriptions/$SUB/resourceGroups/rg-gestaofacil
  terraform import azurerm_linux_virtual_machine.vm /subscriptions/$SUB/resourceGroups/rg-gestaofacil/providers/Microsoft.Compute/virtualMachines/gestaofacil
  # ...e assim por diante para vnet, subnet, nsg, public_ip, nic e a associação.
  terraform plan   # objetivo: nenhuma destruição. Só então considere aplicar.
  ```

## Escopo: infra, não configuração da VM

O Terraform provisiona a **infraestrutura** (VM, rede, firewall). O que roda
*dentro* da VM (instalar Docker, clonar o repo, `.env`, subir os containers,
o self-hosted runner do CI) fica de fora — isso é *configuração*, território de
Ansible/cloud-init/scripts. Manter a fronteira clara é boa prática: Terraform
cuida do "onde", o compose + CI cuidam do "o quê roda dentro".

Um próximo incremento natural seria um `custom_data`/cloud-init na VM instalando
o Docker no primeiro boot — mas aí o site precisaria dos segredos do `.env`, que
não moram no git. Por isso fica documentado como evolução, não automatizado.

## Diferenças conscientes vs. a infra atual

- **Região única.** A RG atual tem metadados em `westus2` por acaso da criação,
  com os recursos em `northcentralus`. Aqui tudo usa uma `location` só
  (`northcentralus`) — mais limpo e sem efeito prático.
- **Nome da NIC** padronizado (`gestaofacil-nic`) em vez do sufixo aleatório que
  o portal gerou (`gestaofacil782`).
