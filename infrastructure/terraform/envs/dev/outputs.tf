output "resource_group_name" { value = azurerm_resource_group.rg.name }
output "aks_name" { value = module.aks.name }
output "acr_login_server" { value = module.acr.login_server }
output "postgres_fqdn" { value = module.postgres.fqdn }
output "redis_hostname" { value = module.redis.hostname }
output "keyvault_uri" { value = module.keyvault.vault_uri }
