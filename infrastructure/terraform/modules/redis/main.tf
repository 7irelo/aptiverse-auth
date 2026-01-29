resource "azurerm_redis_cache" "redis" {
  name                = var.name
  location            = var.location
  resource_group_name = var.resource_group_name

  capacity = var.capacity
  family   = var.family
  sku_name = var.sku_name

  minimum_tls_version = "1.2"
  tags                = var.tags
}
