# =============================================================================
# Infra da GestaoFacil na Azure, como código.
#
# Espelha exatamente o que hoje roda no ar (VM única com 3 containers: MySQL,
# app .NET e Caddy). Ordem de leitura sugerida: Resource Group -> rede
# (VNet/subnet) -> firewall (NSG) -> IP público -> NIC -> VM.
#
# Terraform descobre sozinho a ordem de criação pelas referências entre recursos
# (ex.: a NIC referencia a subnet, então a subnet é criada antes). Você não
# ordena nada à mão — só declara o que depende de quê.
# =============================================================================

# --- Resource Group: a "pasta" que agrupa todos os recursos ------------------
resource "azurerm_resource_group" "rg" {
  name     = "rg-${var.prefixo}"
  location = var.location
  tags     = var.tags
}

# --- Rede virtual + subnet: a "LAN privada" da VM ----------------------------
resource "azurerm_virtual_network" "vnet" {
  name                = "${var.prefixo}-vnet"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  address_space       = ["10.1.0.0/16"] # 65k IPs privados possíveis
  tags                = var.tags
}

resource "azurerm_subnet" "default" {
  name                 = "default"
  resource_group_name  = azurerm_resource_group.rg.name
  virtual_network_name = azurerm_virtual_network.vnet.name
  address_prefixes     = ["10.1.1.0/24"] # a VM pega um IP daqui (ex.: 10.1.1.4)
}

# --- NSG: o firewall da VM (quem entra em quais portas) ----------------------
# Menor privilégio: SSH só do seu IP; 80/443 abertos pro mundo (é um site
# público). As portas internas (8080 do app, 3306 do MySQL, 6379 do Redis)
# NÃO aparecem aqui de propósito — nunca são expostas ao host, só o Caddy fala
# com o mundo.
resource "azurerm_network_security_group" "nsg" {
  name                = "${var.prefixo}-nsg"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  tags                = var.tags

  security_rule {
    name                       = "SSH"
    priority                   = 300 # menor número = avaliado primeiro
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "22"
    source_address_prefix      = var.meu_ip_ssh # só o seu IP
    destination_address_prefix = "*"
  }

  security_rule {
    name                       = "HTTP"
    priority                   = 320
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "80" # Caddy usa pra o desafio do Let's Encrypt e o 308 -> HTTPS
    source_address_prefix      = "*"
    destination_address_prefix = "*"
  }

  security_rule {
    name                       = "HTTPS"
    priority                   = 340
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "443"
    source_address_prefix      = "*"
    destination_address_prefix = "*"
  }
}

# --- IP público estático + FQDN gratuito -------------------------------------
# Static: o IP não muda se a VM reiniciar (senão o DNS/registro A quebraria).
# domain_name_label emite <label>.<location>.cloudapp.azure.com de graça — é o
# domínio que o Caddy usa pra o certificado Let's Encrypt.
resource "azurerm_public_ip" "pip" {
  name                = "${var.prefixo}-ip"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  allocation_method   = "Static"
  sku                 = "Standard" # exigido pra IP estático em VMs modernas
  domain_name_label   = var.dns_label
  tags                = var.tags
}

# --- NIC: a "placa de rede" que liga a VM à subnet e ao IP público -----------
resource "azurerm_network_interface" "nic" {
  name                = "${var.prefixo}-nic"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  tags                = var.tags

  ip_configuration {
    name                          = "ipconfig1"
    subnet_id                     = azurerm_subnet.default.id
    private_ip_address_allocation = "Dynamic" # a Azure escolhe o IP interno
    public_ip_address_id          = azurerm_public_ip.pip.id
  }
}

# Liga o firewall (NSG) à placa de rede (NIC). Na Azure o NSG é um recurso
# separado que você "associa" — não é atributo da NIC.
resource "azurerm_network_interface_security_group_association" "nic_nsg" {
  network_interface_id      = azurerm_network_interface.nic.id
  network_security_group_id = azurerm_network_security_group.nsg.id
}

# --- A VM Linux --------------------------------------------------------------
# disable_password_authentication = true -> só entra por chave SSH, nunca senha.
# É a mesma imagem Ubuntu 22.04 LTS que está no ar.
resource "azurerm_linux_virtual_machine" "vm" {
  name                  = var.prefixo
  location              = azurerm_resource_group.rg.location
  resource_group_name   = azurerm_resource_group.rg.name
  size                  = var.vm_size
  admin_username        = var.admin_username
  network_interface_ids = [azurerm_network_interface.nic.id]
  tags                  = var.tags

  admin_ssh_key {
    username   = var.admin_username
    public_key = var.ssh_public_key
  }

  os_disk {
    caching              = "ReadWrite"
    storage_account_type = var.os_disk_type
    disk_size_gb         = var.os_disk_size_gb
  }

  source_image_reference {
    publisher = "canonical"
    offer     = "ubuntu-22_04-lts"
    sku       = "server"
    version   = "latest"
  }

  disable_password_authentication = true
}
