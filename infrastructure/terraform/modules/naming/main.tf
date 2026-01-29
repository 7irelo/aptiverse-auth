variable "project" { type = string }      # e.g. aptiverse
variable "env" { type = string }          # dev/prod
variable "location_short" { type = string } # e.g. "weu" or "sez"

locals {
  prefix = "${var.project}-${var.env}-${var.location_short}"
}

output "prefix" { value = local.prefix }
