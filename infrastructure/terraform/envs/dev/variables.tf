variable "project" { type = string }
variable "env" { type = string }
variable "location" { type = string }
variable "location_short" { type = string }
variable "tags" {
  type    = map(string)
  default = {}
}

# Network
variable "address_space" { type = list(string) }
variable "aks_subnet_cidr" { type = string }
variable "private_endpoints_subnet_cidr" { type = string }

# AKS
variable "private_cluster_enabled" { 
    type = bool 
    default = false 
}
variable "node_vm_size" { 
    type = string 
    default = "Standard_D4s_v5" 
}
variable "node_count" { 
    type = number 
    default = 2 
}

# Data
variable "pg_admin_user" { type = string }
variable "pg_admin_password" { 
    type = string 
    sensitive = true 
}

# Globally unique names
variable "storage_account_name" { type = string }
