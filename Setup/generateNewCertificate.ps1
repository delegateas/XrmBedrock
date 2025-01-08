# This script will generate a self-signed certificate and export it as a PFX file with the provided password
# To call this script, run the following command in administrator powershell
# Set-ExecutionPolicy Bypass -Scope Process
# .\generateNewCertificate.ps1 -name "myplugin" -friendlyName "My Plugin Certificate" -password "mypluginpassword"
# Store the password in the Plugins.csproj
# Use the thumbprint the created federated identity script

param(
    [Parameter(Mandatory=$true)][string]$name,
    [Parameter(Mandatory=$true)][string]$friendlyName,
    [Parameter(Mandatory=$true)][string]$password,
    [Parameter(Mandatory=$true)][string]$environmentId,
    [Parameter(Mandatory=$true)][string]$appId
)

# 1. Generate a self-signed certificate
$cert = New-SelfSignedCertificate -NotAfter (Get-Date).AddYears(100) -Subject "CN=$name, O=corp, C=$name.com" -DnsName "www.$name.com" -Type CodeSigning -KeyUsage DigitalSignature -CertStoreLocation Cert:\CurrentUser\My -FriendlyName $friendlyName

# Note: The cert object contains a Thumbprint property we will use for the configuration of the federated credentials of the managed identity so keep it available

# 2. Set a password for the private key (optional)
$pw = ConvertTo-SecureString -String $password -Force -AsPlainText

# 3. Export the certificate as a PFX file
$certificatePath = "$PSScriptRoot\..\plugincert.pfx"
Export-PfxCertificate -Cert $cert -FilePath $certificatePath -Password $pw

# 4. Write the thumbprint of the certificate
$thumbprint = $cert.Thumbprint

# 5. Generate the issuer URL based on the environment ID.
$environmentIdPrefix = $environmentId.Substring(0, $environmentId.Length - 2).Replace("-", "")
$environmentIdSuffix = $environmentId.Substring($environmentId.Length - 2)
$issuer = "https://$environmentIdPrefix.$environmentIdSuffix.environment.api.powerplatform.com/sts"

# 6. Generate the subject identifier based on the certificate thumbprint (extracted during the configuration of the certificate) and the environment ID.
$subjectIdentifier = "component:pluginassembly,thumbprint:$certificateThumbprint,environment:$environmentId"

# 7. Configure federated identity credentials for the application registration.
Write-Host ""
Write-Host "##################################################################"
Write-Host "# Information to be used in the federated identity configuration #"
Write-Host "##################################################################"
Write-Host ""
Write-Host "Issuer: $issuer"
Write-Host ""
Write-Host "Subject: $subjectIdentifier"
Write-Host ""
Write-Host "Name (Write the environment name): Dataverse-"
Write-Host ""
Write-Host @("Audience (remember to edit): api://azureadtokenexchange")