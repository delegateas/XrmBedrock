# Prints the federated credential information needed for Dataverse Managed Identity setup.
# Loads an existing plugincert.pfx and computes the issuer, subject, thumbprint, and hash.
#
# Usage:
#   pwsh Setup/printFederatedCredentials.ps1 -password "yourCertPassword" -environmentId "env-guid" -tenantId "tenant-guid"

param(
    [Parameter(Mandatory=$true)][string]$password,
    [Parameter(Mandatory=$true)][string]$environmentId,
    [Parameter(Mandatory=$true)][string]$tenantId
)

$pfxPath = Join-Path $PSScriptRoot ".." "plugincert.pfx"
$resolvedPath = Resolve-Path $pfxPath -ErrorAction SilentlyContinue
if (-not $resolvedPath) {
    Write-Error "Certificate file not found: $pfxPath"
    exit 1
}

$cert = [System.Security.Cryptography.X509Certificates.X509Certificate2]::new(
    $resolvedPath.Path, $password,
    [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::EphemeralKeySet)

$thumbprint = $cert.Thumbprint

# Compute SHA256 hash of the DER-encoded certificate (cross-platform, no certutil)
$sha256 = [System.Security.Cryptography.SHA256]::Create()
$hashBytes = $sha256.ComputeHash($cert.RawData)
$hash = [System.BitConverter]::ToString($hashBytes).Replace("-", "").ToLowerInvariant()
$sha256.Dispose()

# Generate the issuer URL
$issuer = "https://login.microsoftonline.com/${tenantId}/v2.0"

# Encode the tenant ID: GUID -> bytes -> Base64URL
$tenantGuid = [System.Guid]::Parse($tenantId)
$tenantBytes = $tenantGuid.ToByteArray()
$base64 = [System.Convert]::ToBase64String($tenantBytes)
$encodedTenantId = $base64.Replace('+', '-').Replace('/', '_').TrimEnd('=')

# Generate the subject identifier
$subjectIdentifier = "/eid1/c/pub/t/${encodedTenantId}/a/qzXoWDkuqUa3l6zM5mM0Rw/n/plugin/e/${environmentId}/h/${hash}"

$cert.Dispose()

Write-Host ""
Write-Host "##################################################################"
Write-Host "# Information to be used in the federated identity configuration #"
Write-Host "##################################################################"
Write-Host ""
Write-Host "Certificate Path: $($resolvedPath.Path)"
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
