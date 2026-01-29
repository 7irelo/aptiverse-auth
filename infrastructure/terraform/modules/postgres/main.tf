resource "azurerm_postgresql_flexible_server" "pg" {
  name                   = var.name
  resource_group_name    = var.resource_group_name
  location               = var.location
  version                = var.postgres_version
  administrator_login    = var.admin_user
  administrator_password = var.admin_password

  sku_name   = var.sku_name
  storage_mb = var.storage_mb

  backup_retention_days = 7
  tags                  = var.tags
}
