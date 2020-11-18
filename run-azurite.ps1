$var = Get-Command npm
if ($var.Length -eq 0)
{
    Write-Host "NPM is not installed."
    Pause
    return
}

$var = Get-Command azurite
if ($var.Length -eq 0)
{
    npm install -g azurite
}
else
{
    Write-Host "Azurite already installed. Skipping installation."
}

$scriptDir = Split-Path $script:MyInvocation.MyCommand.Path

$azuriteDir = Join-Path -Path $scriptDir -ChildPath "azurite"
$azuriteLogDir = Join-Path -Path $azuriteDir -ChildPath "debug.log"

if (-not (Test-Path $azuriteDir))
{
    New-Item -Path $scriptDir -Name "azurite" -ItemType "Directory"
}

Write-Host "Starting Azurite at $azuriteDir."

try
{
    azurite --location $azuriteDir --debug $azuriteLogDir
}
catch
{
    Write-Error $_.Exception.ToString()
    Write-Host "The above error occurred."
}
finally
{
    Write-Host "Stopped Azurite."
    pause
}