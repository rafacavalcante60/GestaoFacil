# Versões fixadas: Terraform e o provider da Azure.
# Fixar a versão é boa prática de IaC — garante que "roda igual" hoje e daqui a
# um ano, sem uma atualização do provider mudar o comportamento de repente.
terraform {
  required_version = ">= 1.5"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0" # ~> 4.0 = qualquer 4.x, mas nunca sobe pra 5.x sozinho
    }
  }
}

provider "azurerm" {
  # subscription_id vem da env var ARM_SUBSCRIPTION_ID ou do `az login` ativo.
  # features {} é obrigatório mesmo vazio — o provider exige o bloco.
  features {}
}
