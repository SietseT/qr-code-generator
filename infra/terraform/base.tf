resource "azurerm_resource_group" "resource_group" {
  name     = "rg-${local.base_resource_name}"
  location = local.location
}

/*
  Service Bus
*/

resource "azurerm_servicebus_namespace" "service_bus" {
  name                = "sb-${local.base_resource_name}"
  location            = azurerm_resource_group.resource_group.location
  resource_group_name = azurerm_resource_group.resource_group.name
  sku                 = "Basic"
}

resource "azurerm_servicebus_namespace_authorization_rule" "api_send" {
  name         = "ApiSend"
  namespace_id = azurerm_servicebus_namespace.service_bus.id

  listen = false
  send   = true
  manage = false
}

resource "azurerm_servicebus_namespace_authorization_rule" "generator_read" {
  name         = "GeneratorRead"
  namespace_id = azurerm_servicebus_namespace.service_bus.id

  listen = true
  send   = false
  manage = false
}

resource "azurerm_servicebus_queue" "wifi_queue" {
  name                  = "wifi"
  namespace_id          = azurerm_servicebus_namespace.service_bus.id
  max_size_in_megabytes = 1024
}

/*
  Storage account
*/

resource "azurerm_storage_account" "storage_account" {
  name                      = "sa${local.application}${var.environment_name}"
  resource_group_name       = azurerm_resource_group.resource_group.name
  location                  = azurerm_resource_group.resource_group.location
  account_tier              = "Standard"
  account_replication_type  = "LRS"
  
  min_tls_version           = "TLS1_2"
}

/*
  SignalR
*/
resource "azurerm_signalr_service" "signalr" {
  name                = "sr-${local.base_resource_name}"
  location            = azurerm_resource_group.resource_group.location
  resource_group_name = azurerm_resource_group.resource_group.name

  sku {
    name     = "Free_F1"
    capacity = 1
  }

  cors {
    allowed_origins = ["*"]
  }

  connectivity_logs_enabled = true
  messaging_logs_enabled    = true
  service_mode              = "Serverless"

  lifecycle {
    ignore_changes = [ upstream_endpoint ]
  }
}

/*
  Service plans
*/
resource "azurerm_service_plan" "consumption" {
  name                = "asp-${local.application}-functions-${var.environment_name}"
  resource_group_name = azurerm_resource_group.resource_group.name
  location            = azurerm_resource_group.resource_group.location
  os_type             = "Linux"
  sku_name            = "Y1" // Consumption plan
}

resource "azurerm_service_plan" "apps" {
  name                = "asp-${local.application}-apps-${var.environment_name}"
  resource_group_name = azurerm_resource_group.resource_group.name
  location            = azurerm_resource_group.resource_group.location
  os_type             = "Windows"
  sku_name            = "F1" // Free
}

/*
  Function Apps
*/
locals {
  hub_function_name = "func-${local.application}-hub-${var.environment_name}"
  hub_upstream      = "https://${local.hub_function_name}.azurewebsites.net/runtime/webhooks/signalr?code=${data.azurerm_function_app_host_keys.hub.signalr_extension_key}"
}

resource "azurerm_linux_function_app" "hub" {
  
  name                        = local.hub_function_name
  resource_group_name         = azurerm_resource_group.resource_group.name
  location                    = azurerm_resource_group.resource_group.location

  storage_account_name        = azurerm_storage_account.storage_account.name
  storage_account_access_key  = azurerm_storage_account.storage_account.primary_access_key
  functions_extension_version = "~4"
  service_plan_id             = azurerm_service_plan.consumption.id

  https_only                  = true
  
  app_settings = {
    WEBSITE_RUN_FROM_PACKAGE  = ""
  }
 
  connection_string {
    name    = "AzureSignalRConnectionString"
    type    = "Custom"
    value   = azurerm_signalr_service.signalr.primary_connection_string
  }  

  site_config {
    ftps_state                = "Disabled"

    application_stack {
      dotnet_version  = "6.0"
    }
  }

  lifecycle {
    ignore_changes = [ app_settings["WEBSITE_RUN_FROM_PACKAGE"] ]
  }
}

data "azurerm_function_app_host_keys" "hub" {
  name                = local.hub_function_name
  resource_group_name = azurerm_resource_group.resource_group.name
}

resource "null_resource" "signalr_upstream" {  
  triggers = {
    signalr_id = azurerm_signalr_service.signalr.id
    hub_url    = local.hub_upstream
  }

  provisioner "local-exec" {
    interpreter = ["PowerShell", "-Command"]
    command     = <<EOT
      $resourceName = "${azurerm_signalr_service.signalr.name}"
      $resourceGroup = "${azurerm_resource_group.resource_group.name}"

      $signalr = (az signalr show --name $resourceName --resource-group $resourceGroup) | ConvertFrom-Json
      if(!$signalR) {
          throw "SignalR service with name '$resourceName' not found."
      }


      $upstreams = (az signalr upstream list --name $resourceName --resource-group $resourceGroup --query "templates") | ConvertFrom-Json
      if($upstreams.length -gt 0) {
          Write-Host "Clearing upstreams..."
          az signalr upstream clear --name $resourceName --resource-group $resourceGroup
      }

      Write-Host "Adding upstream..."
      az signalr upstream update `
                      --name $resourceName `
                      --resource-group $resourceGroup `
                      --template url-template=${local.hub_upstream}
    EOT
  }

  depends_on = [
    data.azurerm_function_app_host_keys.hub,
    azurerm_linux_function_app.hub,
    azurerm_signalr_service.signalr
  ]
}