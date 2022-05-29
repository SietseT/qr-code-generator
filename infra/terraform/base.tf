resource "azurerm_resource_group" "resource-group" {
  name     = "rg-qrgenerator-${var.environment_name}"
  location = "westeurope"
}