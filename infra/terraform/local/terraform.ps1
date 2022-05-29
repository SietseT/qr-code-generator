param ($command)

$localEnvFile = "local.env"

if(!(Test-Path -Path $localEnvFile)) {
  Throw "local.env not found."
}

If($command -ne "plan" -and $command -ne "apply") {
  Write-Error -Message "Command was not 'plan' or 'apply'." -ErrorAction Stop 
}

switch -File $localEnvFile {
  default {
    $name, $value = $_.Trim() -split '=', 2
    if ($name -and $name[0] -ne '#') { # ignore blank and comment lines.
      Set-Item "Env:$name" $value
    }
  }
}

Push-Location "../"

try 
{
  terraform init `
    -backend-config="storage_account_name=$Env:TF_VAR_azure_state_storage_account" `
    -backend-config="key=$($Env:TF_VAR_state_filename).tfstate" `
    -backend-config="access_key=$Env:TF_VAR_azure_stage_storage_account_accesskey"

  Write-Host ""

  if($command -eq "plan") {
    Write-Host "Planning infrastructure changes..."
    terraform plan -out=local/tfplan
  }
  elseif($command -eq "apply") {    
    Write-Host "Applying infrastructure changes..."
    terraform apply local/tfplan
  }
}
catch { }

Pop-Location