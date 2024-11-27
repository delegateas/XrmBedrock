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
    [String] $DataverseClientId, 
    [Parameter(Mandatory = $false)]
    [String] $DataverseClientSecret,
    [Parameter(Mandatory = $false)]
    [String] $CvrUsername, 
    [Parameter(Mandatory = $false)]
    [String] $CvrPassword,
    [Parameter(Mandatory = $false)]
    [String] $WebAppClientSecret,
    [Parameter(Mandatory = $false)]
    [String] $ManagedIdentitiesForExternal,
    [Parameter(Mandatory = $false)]
    [Switch] $what_if = $false,
    [Parameter(Mandatory = $false)]
    [Boolean] $cprDisabled, 
    [Parameter(Mandatory = $false)]
    [String] $cprKey,
    [Parameter(Mandatory = $false)]
    [String] $cprCustomerNumber, 
    [Parameter(Mandatory = $false)]
    [String] $cprConnectionString,
    [Parameter(Mandatory = $false)]
    [String] $cprFTPUser, 
    [Parameter(Mandatory = $false)]
    [String] $cprFTPPassword,
    [Parameter(Mandatory = $false)]
    [String] $cprDeployPackageUrl,
    [Parameter(Mandatory = $false)]
    [String] $farPayApiKey,
    [Parameter(Mandatory = $false)]
    [String] $DMAzureSqlConnectionString,
    [Parameter(Mandatory = $false)]
    [Boolean] $getPaymentsFromFarpayDisabled, 
    [Parameter(Mandatory = $false)]
    [String] $existingAppClientId,
    [Parameter(Mandatory = $false)]
    [String] $existingAppClientObjectId
)

$Secure_DataverseUrl = ConvertTo-SecureString $DataverseUrl -AsPlainText -Force
$Secure_DataverseClientId = ConvertTo-SecureString $DataverseClientId -AsPlainText -Force
$Secure_DataverseClientSecret = ConvertTo-SecureString $DataverseClientSecret -AsPlainText -Force
$Secure_CvrUsername = ConvertTo-SecureString $CvrUsername -AsPlainText -Force
$Secure_CvrPassword = ConvertTo-SecureString $CvrPassword -AsPlainText -Force
$Secure_WebAppClientSecret = ConvertTo-SecureString $WebAppClientSecret -AsPlainText -Force
$Secure_cprFTPUser = ConvertTo-SecureString $cprFTPUser -AsPlainText -Force
$Secure_cprFTPPassword = ConvertTo-SecureString $cprFTPPassword -AsPlainText -Force
$Secure_farPayApiKey = ConvertTo-SecureString $farPayApiKey -AsPlainText -Force
$Secure_DMAzureSqlConnectionString = ConvertTo-SecureString $DMAzureSqlConnectionString -AsPlainText -Force

$deployment = @{
    'ResourceGroupName'             = $resourceGroupName
    'mode'                          = 'Incremental'
    'TemplateFile'                  = $template 
    'TemplateParameterFile'         = $Parameters
    'DataverseUrl'                  = $Secure_DataverseUrl
    'DataverseClientId'             = $Secure_DataverseClientId
    'DataverseClientSecret'         = $Secure_DataverseClientSecret
    'CvrUsername'                   = $Secure_CvrUsername
    'CvrPassword'                   = $Secure_CvrPassword
    'WebAppClientSecret'            = $Secure_WebAppClientSecret
    'ManagedIdentitiesForExternal'  = $ManagedIdentitiesForExternal
    'WhatIf'                        = $what_if
    'cprDisabled'                   = $cprDisabled
    'cprKey'                        = $cprKey
    'cprCustomerNumber'             = $cprCustomerNumber
    'cprConnectionString'           = $cprConnectionString
    'cprFTPUser'                    = $Secure_cprFTPUser
    'cprFTPPassword'                = $Secure_cprFTPPassword
    'cprDeployPackageUrl'           = $cprDeployPackageUrl
    'FarPayApiKey'                  = $Secure_farPayApiKey
    'DMAzureSqlConnectionString'    = $Secure_DMAzureSqlConnectionString
    'getPaymentsFromFarpayDisabled' = $getPaymentsFromFarpayDisabled
    'ExistingAppClientId'           = $existingAppClientId
    'ExistingAppClientObjectId'     = $existingAppClientObjectId
}

New-AzResourceGroupDeployment @deployment
