resource "azurerm_key_vault" "kv" {
  name                       = replace(var.name, "_", "-")
  location                   = var.location
  resource_group_name        = var.resource_group_name
  tenant_id                  = var.tenant_id
  sku_name                   = "standard"
  purge_protection_enabled   = true
  soft_delete_retention_days = 7
  tags                       = var.tags

  network_acls {
    default_action = "Allow"
    bypass         = "AzureServices"
  }
}
