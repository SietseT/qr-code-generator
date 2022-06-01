locals {
  location           = "westeurope"
  application        = "qrgenerator"
  base_resource_name = "${local.application}-${var.environment_name}"
}