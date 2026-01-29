project        = "aptiverse"
env            = "dev"
location       = "South Africa North"
location_short = "san"

tags = {
  app = "aptiverse"
  env = "dev"
}

address_space                 = ["10.10.0.0/16"]
aks_subnet_cidr               = "10.10.1.0/24"
private_endpoints_subnet_cidr = "10.10.2.0/24"

private_cluster_enabled = false
node_vm_size            = "Standard_D4s_v5"
node_count              = 2

pg_admin_user     = "pgadmin"
pg_admin_password = "CHANGE_ME_STRONG_PASSWORD"

# must be globally unique + lowercase + numbers only (3-24 chars)
storage_account_name = "aptiversedevsan001"
