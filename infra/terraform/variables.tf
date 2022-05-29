variable "azure_subscription_id" {
  type = string
}

variable "azure_tenant_id" {
  type = string
}

variable "azure_state_storage_account" {
  type        = string
  description = "Name of storage account in Azure to use as backend for state storage."
}

variable "state_filename" {
  type        = string
  description = "Name of the state file."
}

variable "environment_name" {
  type        = string
  description = "Environment name"

  validation {
    condition     = var.environment_name == "dev" || var.environment_name == "tst" || var.environment_name == "acc" || var.environment_name == "prd"
    error_message = "Environment name must be either: dev, tst, acc or prd."
  }
}