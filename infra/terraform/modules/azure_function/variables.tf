variable "function_app_settings" {
  type        = map(string)
  description = "Function App settings"
  nullable    = true
}
variable "function_connection_strings" {
  type        = list(object({
    name    = string
    type    = string
    value   = string
  }))
  description = "Function App connection strings"  
  nullable    = true
}
variable "function_location" {
  type        = string
  description = "Location of the resource group"
}
variable "function_name" {
  type        = string
  description = "Name of the Azure Function App"
}

variable "resource_group_name" {
  type        = string
  description = "Name of the resource group"
}

variable "service_plan_id" {
  type        = string
  description = "Id of the service plan that the Function App will use"
}

variable "storage_account_name" {
  type        = string
  description = "Name of the storage account used by the Function"
}
variable "storage_account_connection_string" {
  type        = string
  description = "Connection string of the storage account used by the Function"
}

