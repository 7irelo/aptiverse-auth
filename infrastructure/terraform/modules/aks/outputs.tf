output "id" { value = azurerm_kubernetes_cluster.aks.id }
output "name" { value = azurerm_kubernetes_cluster.aks.name }
output "kube_config" {
  value     = azurerm_kubernetes_cluster.aks.kube_config_raw
  sensitive = true
}
output "oidc_issuer_url" { value = azurerm_kubernetes_cluster.aks.oidc_issuer_url }
