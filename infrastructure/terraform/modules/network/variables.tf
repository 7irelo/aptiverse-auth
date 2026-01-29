variable "name" { type = string }
variable "location" { type = string }
variable "resource_group_name" { type = string }
variable "address_space" { type = list(string) }
variable "aks_subnet_cidr" { type = string }
variable "private_endpoints_subnet_cidr" { type = string }
variable "tags" { 
    type = map(string) 
    default = {} 
}
