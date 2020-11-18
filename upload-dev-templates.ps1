if ($PSVersionTable.PSEdition -eq 'Desktop' -and (Get-Module -Name AzureRM -ListAvailable))
{
    Write-Warning -Message ('Az module not installed. Having both the AzureRM and ' +
      'Az modules installed at the same time is not supported.')
}
elseif (Get-Module -Name Az -ListAvailable)
{
    Write-Host "Az module already installed. Skipping installation."
}
else
{
    Install-Module -Name Az -AllowClobber -Scope CurrentUser
}

$containerName = "mercury"
$context = New-AzStorageContext -Local

$container = Get-AzStorageContainer -Name $containerName -Context $context

if (!$container)
{
    New-AzStorageContainer -Name $containerName -Permission Off -Context $context
}

## upload a file to the default account (inferred) access tier
Set-AzStorageBlobContent -File "Templates\liquid-example\template-name\template.html"     -Container $containerName -Blob "liquid-example\template-name\template.html"     -Context $context -Force
Set-AzStorageBlobContent -File "Templates\liquid-example\template-name\metadata.json"     -Container $containerName -Blob "liquid-example\template-name\metadata.json"     -Context $context -Force
Set-AzStorageBlobContent -File "Templates\handlebars-example\template-name\template.html" -Container $containerName -Blob "handlebars-example\template-name\template.html" -Context $context -Force
Set-AzStorageBlobContent -File "Templates\handlebars-example\template-name\metadata.json" -Container $containerName -Blob "handlebars-example\template-name\metadata.json" -Context $context -Force

pause