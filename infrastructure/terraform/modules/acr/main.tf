resource "azurerm_container_registry" "acr" {
  name                = replace(var.name, "-", "")
  resource_group_name = var.resource_group_name
  location            = var.location
  sku                 = var.sku
  admin_enabled       = false
  tags                = var.tags
}
