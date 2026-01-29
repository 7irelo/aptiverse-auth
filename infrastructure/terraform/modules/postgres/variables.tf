variable "name" { type = string }
variable "location" { type = string }
variable "resource_group_name" { type = string }
variable "admin_user" { type = string }
variable "admin_password" { 
    type = string 
    sensitive = true 
}
variable "sku_name" { 
    type = string 
    default = "GP_Standard_D2s_v3" 
}
variable "storage_mb" { 
    type = number 
    default = 32768 
}
variable "postgres_version" {
  type    = string
  default = "16"
}
variable "tags" { 
    type = map(string) 
    default = {} 
}
