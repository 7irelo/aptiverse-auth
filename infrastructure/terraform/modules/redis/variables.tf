variable "name" { type = string }
variable "location" { type = string }
variable "resource_group_name" { type = string }
variable "capacity" { 
    type = number 
    default = 1 
}
variable "family" { 
    type = string 
    default = "C" 
}
variable "sku_name" { 
    type = string 
    default = "Standard" 
}
variable "tags" { 
    type = map(string) 
    default = {} 
}
