# Terraform - Azure AKS Platform

This provisions:
- Resource Group(s)
- VNet/Subnets
- AKS (private or public API toggle)
- ACR
- Key Vault
- Postgres Flexible Server
- Azure Cache for Redis
- Storage Account (Blob)
- Log Analytics + Azure Monitor integration

State:
- Stored in an Azure Storage Account created by `bootstrap/`.

CI/CD:
- GitHub Actions workflows for fmt/validate/plan and apply.
- Uses Azure login via OIDC (recommended).