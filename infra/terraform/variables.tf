# Todas as "peças que mudam" da infra ficam aqui como variáveis. Assim o mesmo
# código serve pra recriar em outra região, outro tamanho de VM ou outro nome,
# sem mexer no main.tf. Os defaults refletem a infra real que está no ar hoje.

variable "prefixo" {
  description = "Prefixo/nome base de todos os recursos (vira gestaofacil-vm, gestaofacil-nsg, etc.)."
  type        = string
  default     = "gestaofacil"
}

variable "location" {
  description = "Região da Azure onde tudo é criado. A infra atual vive em North Central US (perto de onde o FQDN gratuito é emitido)."
  type        = string
  default     = "northcentralus"
}

variable "vm_size" {
  description = "Tamanho da VM. B2ats_v2 é a família 'burstable AMD' gratuita por 12 meses no Azure for Students."
  type        = string
  default     = "Standard_B2ats_v2"
}

variable "admin_username" {
  description = "Usuário administrador Linux da VM (login por SSH)."
  type        = string
  default     = "azureuser"
}

variable "ssh_public_key" {
  description = "Chave PÚBLICA SSH autorizada a entrar na VM. Chave pública NÃO é segredo — a privada (.pem) nunca entra aqui. Por padrão lê o par gerado localmente."
  type        = string
  # Se você tiver a .pub ao lado da .pem, aponte pra ela no terraform.tfvars.
  # O default abaixo é a chave que já está autorizada na VM que está no ar.
  default = "ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIBJdXzE505bSaIGny6xcosWsbg/MOJ1kXGfPOfa4iv3F generated-by-azure"
}

variable "meu_ip_ssh" {
  description = "IP público de onde você acessa a VM por SSH (porta 22). É DINÂMICO na casa do Vinas — se o SSH parar de conectar, atualize aqui. 80/443 ficam abertos pra todos."
  type        = string
  default     = "179.134.117.56"
}

variable "dns_label" {
  description = "Rótulo do FQDN gratuito da Azure: <label>.<location>.cloudapp.azure.com. Precisa ser único dentro da região."
  type        = string
  default     = "gestaofacil"
}

variable "os_disk_size_gb" {
  description = "Tamanho do disco do SO em GB."
  type        = number
  default     = 64
}

variable "os_disk_type" {
  description = "Tipo do disco gerenciado. Premium_LRS é SSD premium; StandardSSD_LRS é mais barato se quiser economizar."
  type        = string
  default     = "Premium_LRS"
}

variable "tags" {
  description = "Tags aplicadas a todos os recursos — ajudam a rastrear custo e dono no portal."
  type        = map(string)
  default = {
    projeto    = "gestaofacil"
    ambiente   = "producao"
    gerido_por = "terraform"
  }
}
