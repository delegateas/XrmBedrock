# This script will generate a self-signed certificate and export it as a PFX file with the provided password
# OR it can use an existing PFX file to recalculate the issuer and subject
# To call this script, run the following command in administrator powershell
# Set-ExecutionPolicy Bypass -Scope Process
# 
# Generate new certificate:
# .\generateNewCertificate.ps1 -name "myplugin" -friendlyName "My Plugin Certificate" -password "mypluginpassword" -environmentId "env123" -tenantId "b654096d-cf96-4427-a4b7-ba9c91c8d72e"
#
# Use existing certificate:
# .\generateNewCertificate.ps1 -existingPfxPath "C:\path\to\cert.pfx" -password "mypluginpassword" -environmentId "env123" -tenantId "b654096d-cf96-4427-a4b7-ba9c91c8d72e"
# 
# Store the password in the Plugins.csproj
# Use the thumbprint the created federated identity script

param(
    [Parameter(Mandatory=$false)][string]$name,
    [Parameter(Mandatory=$false)][string]$friendlyName,
    [Parameter(Mandatory=$true)][string]$password,
    [Parameter(Mandatory=$true)][string]$environmentId,
    [Parameter(Mandatory=$true)][string]$tenantId,
    [Parameter(Mandatory=$false)][string]$existingPfxPath
)

# Validate that either existingPfxPath OR (name and friendlyName) are provided
if ($existingPfxPath) {
    # Resolve relative path to absolute path
    $resolvedPath = Resolve-Path $existingPfxPath -ErrorAction SilentlyContinue
    if (-not $resolvedPath) {
        Write-Error "The specified PFX file does not exist: $existingPfxPath"
        exit 1
    }
    $existingPfxPath = $resolvedPath.Path
    Write-Host "Using existing certificate from: $existingPfxPath"
} elseif ($name -and $friendlyName) {
    Write-Host "Generating new certificate..."
} else {
    Write-Error "You must provide either -existingPfxPath OR both -name and -friendlyName"
    exit 1
}

$cert = $null
$certificatePath = ""

if ($existingPfxPath) {
    # Load existing certificate from PFX file
    $pw = ConvertTo-SecureString -String $password -Force -AsPlainText
    $cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($existingPfxPath, $pw)
    $certificatePath = $existingPfxPath
    Write-Host "Certificate loaded successfully"
} else {
    # 1. Generate a self-signed certificate
    $cert = New-SelfSignedCertificate -NotAfter (Get-Date).AddYears(100) -Subject "CN=$name, O=corp, C=$name.com" -DnsName "www.$name.com" -Type CodeSigning -KeyUsage DigitalSignature -CertStoreLocation Cert:\CurrentUser\My -FriendlyName $friendlyName

    # Note: The cert object contains a Thumbprint property we will use for the configuration of the federated credentials of the managed identity so keep it available

    # 2. Set a password for the private key (optional)
    $pw = ConvertTo-SecureString -String $password -Force -AsPlainText

    # 3. Export the certificate as a PFX file
    $certificatePath = "$PSScriptRoot\..\..\..\plugincert.pfx"
    Export-PfxCertificate -Cert $cert -FilePath $certificatePath -Password $pw
    Write-Host "Certificate generated and exported to: $certificatePath"
}

# 4. Write the thumbprint of the certificate
$thumbprint = $cert.Thumbprint

# 5. Export cer file and get hash
$tempCerPath = "$PSScriptRoot\extracted.cer"
$cert.RawData | Set-Content -Encoding Byte -Path $tempCerPath
$certUtilOutput = certutil -hashfile $tempCerPath SHA256
$hash = ($certUtilOutput | Select-String -Pattern "^[0-9a-fA-F]{64}$").ToString().Trim()
Remove-Item $tempCerPath -ErrorAction SilentlyContinue

# 6. Generate the issuer URL based on the environment ID.
$environmentIdPrefix = $environmentId.Substring(0, $environmentId.Length - 2).Replace("-", "")
$environmentIdSuffix = $environmentId.Substring($environmentId.Length - 2)
$issuer = "https://login.microsoftonline.com/${tenantId}/v2.0"

# 7. Encode the tenant ID
# Convert GUID → Hex (byte array) → Base64URL
$tenantGuid = [System.Guid]::Parse($tenantId)
$tenantBytes = $tenantGuid.ToByteArray()
$base64 = [System.Convert]::ToBase64String($tenantBytes)
# Convert standard Base64 to Base64URL (replace +/= with -_)
$encodedTenantId = $base64.Replace('+', '-').Replace('/', '_').TrimEnd('=')

# 8. Generate the subject identifier based on the certificate thumbprint (extracted during the configuration of the certificate) and the environment ID.
$subjectIdentifier = "/eid1/c/pub/t/${encodedTenantId}/a/qzXoWDkuqUa3l6zM5mM0Rw/n/plugin/e/${environmentId}/h/${hash}"

# 9. Configure federated identity credentials for the application registration.
Write-Host ""
Write-Host "##################################################################"
Write-Host "# Information to be used in the federated identity configuration #"
Write-Host "##################################################################"
Write-Host ""
Write-Host "Certificate Path: $certificatePath"
Write-Host ""
Write-Host "Thumbprint: $thumbprint"
Write-Host ""
Write-Host "Hash (SHA256): $hash"
Write-Host ""
Write-Host "Encoded Tenant ID: $encodedTenantId"
Write-Host ""
Write-Host "Issuer: $issuer"
Write-Host ""
Write-Host "Subject: $subjectIdentifier"
Write-Host ""
Write-Host "Name (Write the environment name): Dataverse-"
Write-Host ""
Write-Host "Audience (remember to edit): api://AzureADTokenExchange"
Write-Host ""
Write-Host "##################################################################"
 