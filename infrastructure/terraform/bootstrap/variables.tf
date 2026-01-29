variable "location" { type = string }
variable "resource_group_name" { type = string }
variable "storage_account_name" { type = string }
variable "container_name" { type = string }
variable "tags" {
  type    = map(string)
  default = {}
}