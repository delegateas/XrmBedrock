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
    [String] $existingAppClientId,
    [Parameter(Mandatory = $false)]
    [String] $existingAppClientObjectId
)

# Secure strings for sensitive data
$Secure_DataverseUrl = ConvertTo-SecureString $DataverseUrl -AsPlainText -Force

# # Parse ManagedIdentitiesForExternal JSON
# try {
#     $ManagedIdentitiesObject = $ManagedIdentitiesForExternal | ConvertFrom-Json
#     Write-Output "ManagedIdentitiesForExternal parsed successfully."
# } catch {
#     Write-Error "Failed to parse ManagedIdentitiesForExternal as JSON: $_"
# }

# # Example: You can now access the parsed JSON object like this
# foreach ($key in $ManagedIdentitiesObject.PSObject.Properties.Name) {
#     $value = $ManagedIdentitiesObject.$key
#     Write-Output "Key: $key, Value: $value"
# }

# Continue with the deployment
$deployment = @{
    'ResourceGroupName'             = $resourceGroupName
    'mode'                          = 'Incremental'
    'TemplateFile'                  = $template 
    'TemplateParameterFile'         = $Parameters
    'DataverseUrl'                  = $Secure_DataverseUrl
    'ManagedIdentitiesForExternal'  = $ManagedIdentitiesForExternal
    'ExistingAppClientId'           = $existingAppClientId
    'ExistingAppClientObjectId'     = $existingAppClientObjectId
}

# Perform deployment
Test-AzResourceGroupDeployment @deployment -Verbose
