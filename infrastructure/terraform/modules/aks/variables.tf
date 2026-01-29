variable "name" { type = string }
variable "location" { type = string }
variable "resource_group_name" { type = string }

variable "kubernetes_version" { 
    type = string 
    default = null 
}
variable "aks_subnet_id" { type = string }

variable "log_analytics_workspace_id" { type = string }

variable "node_vm_size" { 
    type = string 
    default = "Standard_D4s_v5" 
}
variable "node_count" { 
    type = number 
    default = 2 
}

variable "tags" { 
    type = map(string) 
    default = {} 
}

# Enable private cluster API endpoint (recommended for prod)
variable "private_cluster_enabled" { 
    type = bool 
    default = false 
}

# ACR attachment
variable "acr_id" { type = string }
