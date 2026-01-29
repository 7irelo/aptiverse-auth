location            = "South Africa North"
resource_group_name = "tfstate-rg"

# must be globally unique, lowercase letters+numbers only, 3-24 chars
storage_account_name = "aptiversetfstatesan001"

container_name = "tfstate"

tags = {
  app = "aptiverse"
  env = "shared"
}
