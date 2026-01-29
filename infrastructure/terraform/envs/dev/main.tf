provider "azurerm" {
  features {}
}

data "azurerm_client_config" "current" {}

module "naming" {
  source         = "../../modules/naming"
  project        = var.project
  env            = var.env
  location_short = var.location_short
}

resource "azurerm_resource_group" "rg" {
  name     = "${module.naming.prefix}-rg"
  location = var.location
  tags     = var.tags
}

module "monitor" {
  source              = "../../modules/monitor"
  name                = module.naming.prefix
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  tags                = var.tags
}

module "network" {
  source                         = "../../modules/network"
  name                           = module.naming.prefix
  location                       = var.location
  resource_group_name            = azurerm_resource_group.rg.name
  address_space                  = var.address_space
  aks_subnet_cidr                = var.aks_subnet_cidr
  private_endpoints_subnet_cidr  = var.private_endpoints_subnet_cidr
  tags                           = var.tags
}

module "acr" {
  source              = "../../modules/acr"
  name                = "${module.naming.prefix}-acr"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  tags                = var.tags
}

module "keyvault" {
  source              = "../../modules/keyvault"
  name                = "${module.naming.prefix}-kv"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  tenant_id           = data.azurerm_client_config.current.tenant_id
  tags                = var.tags
}

module "storage" {
  source              = "../../modules/storage"
  name                = var.storage_account_name
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  tags                = var.tags
}

module "postgres" {
  source              = "../../modules/postgres"
  name                = "${module.naming.prefix}-pg"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  admin_user          = var.pg_admin_user
  admin_password      = var.pg_admin_password
  tags                = var.tags
}

module "redis" {
  source              = "../../modules/redis"
  name                = "${module.naming.prefix}-redis"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  tags                = var.tags
}

module "aks" {
  source                     = "../../modules/aks"
  name                       = "${module.naming.prefix}-aks"
  location                   = var.location
  resource_group_name        = azurerm_resource_group.rg.name
  aks_subnet_id              = module.network.aks_subnet_id
  log_analytics_workspace_id = module.monitor.log_analytics_workspace_id
  private_cluster_enabled    = var.private_cluster_enabled
  node_vm_size               = var.node_vm_size
  node_count                 = var.node_count
  acr_id                     = module.acr.id
  tags                       = var.tags
}