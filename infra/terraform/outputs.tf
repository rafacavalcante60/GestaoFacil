# Outputs: o que o Terraform imprime depois do apply. São os dados que você
# precisa pra usar a VM logo em seguida (SSH, apontar o navegador, DNS).

output "ip_publico" {
  description = "IP público estático da VM."
  value       = azurerm_public_ip.pip.ip_address
}

output "fqdn" {
  description = "Endereço DNS gratuito da Azure — é o domínio do site (Caddy emite o cert Let's Encrypt pra ele)."
  value       = azurerm_public_ip.pip.fqdn
}

output "comando_ssh" {
  description = "Comando pronto pra entrar na VM."
  value       = "ssh -i ~/.ssh/gestaofacil_key.pem ${var.admin_username}@${azurerm_public_ip.pip.fqdn}"
}
