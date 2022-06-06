resource "azurerm_linux_function_app" "function" {
  
  name                        = var.function_name
  resource_group_name         = var.resource_group_name
  location                    = var.function_location

  storage_account_name        = var.storage_account_name
  storage_account_access_key  = var.storage_account_connection_string
  functions_extension_version = "~4"
  service_plan_id             = var.service_plan_id

  https_only                  = true
  
  app_settings = {
    WEBSITE_RUN_FROM_PACKAGE  = ""
  }
 
  dynamic "connection_string" {
    for_each = var.function_connection_strings
    content {
      name = connection_string.value["name"]
      type = connection_string.value["type"]
      value = connection_string.value["value"]
    }
  }

  site_config {
    ftps_state = "Disabled"

    application_stack {
      dotnet_version  = "6.0"
    }
  }

  lifecycle {
    ignore_changes = [ app_settings["WEBSITE_RUN_FROM_PACKAGE"] ]
  }
}