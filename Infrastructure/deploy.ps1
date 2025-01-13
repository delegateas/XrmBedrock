[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [String] $resourceGroupName,
    [Parameter(Mandatory = $true)]
    [String] $template,
    [Parameter(Mandatory = $true)]
    [String] $Parameters,    
    [Parameter(Mandatory = $false)]
    [String] $DataverseUrl,
    [Parameter(Mandatory = $false)]
    [String] $ManagedIdentitiesForExternal,
    [Parameter(Mandatory = $false)]
    [Switch] $what_if = $false,
    [Parameter(Mandatory = $false)]
    [String] $existingAppClientId,
    [Parameter(Mandatory = $false)]
    [String] $existingAppClientObjectId
)

$Secure_DataverseUrl = ConvertTo-SecureString $DataverseUrl -AsPlainText -Force

$deployment = @{
    'ResourceGroupName'             = $resourceGroupName
    'mode'                          = 'Incremental'
    'TemplateFile'                  = $template 
    'TemplateParameterFile'         = $Parameters
    'DataverseUrl'                  = $Secure_DataverseUrl
    'ManagedIdentitiesForExternal'  = $ManagedIdentitiesForExternal
    'WhatIf'                        = $what_if
    'ExistingAppClientId'           = $existingAppClientId
    'ExistingAppClientObjectId'     = $existingAppClientObjectId
}

New-AzResourceGroupDeployment @deployment
