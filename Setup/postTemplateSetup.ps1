# Post-template setup script
# Runs as a dotnet new post-action, or manually via: pwsh Setup/postTemplateSetup.ps1

Write-Host "Initializing git repository..."
git init -q

# Generate strong name key (SNK) using cross-platform CSP blob construction.
# Template engine replaces 'xrmbedrock' with the lowercase project name.
Write-Host "Generating strong name key..."
$snkPath = "xrmbedrock.snk"

$rsa = [System.Security.Cryptography.RSA]::Create(1024)
$p = $rsa.ExportParameters($true)

$ms = [System.IO.MemoryStream]::new()
$bw = [System.IO.BinaryWriter]::new($ms)

# BLOBHEADER (8 bytes): type, version, reserved, algorithm
$bw.Write([byte]0x07)           # PRIVATEKEYBLOB
$bw.Write([byte]0x02)           # Version 2
$bw.Write([uint16]0)            # Reserved
$bw.Write([uint32]0x00002400)   # CALG_RSA_SIGN

# RSAPUBKEY (12 bytes): magic, bit length, public exponent
$bw.Write([byte[]]@(0x52, 0x53, 0x41, 0x32))  # "RSA2" magic
$bw.Write([uint32]($p.Modulus.Length * 8))
$pubExp = [uint32]0
for ($i = 0; $i -lt $p.Exponent.Length; $i++) {
    $pubExp = $pubExp -bor ([uint32]$p.Exponent[$p.Exponent.Length - 1 - $i] -shl ($i * 8))
}
$bw.Write($pubExp)

# Key components: convert .NET big-endian arrays to CSP little-endian format
foreach ($component in @($p.Modulus, $p.P, $p.Q, $p.DP, $p.DQ, $p.InverseQ, $p.D)) {
    $le = $component.Clone()
    [Array]::Reverse($le)
    $bw.Write([byte[]]$le)
}

$bw.Flush()
[System.IO.File]::WriteAllBytes($snkPath, $ms.ToArray())

$bw.Dispose()
$ms.Dispose()
$rsa.Dispose()
Write-Host "Generated $snkPath"

# Generate plugin signing certificate (PFX) using cross-platform .NET crypto APIs.
# Template engine replaces 'templatecertpassword' with the cert password and 'xrmbedrock' with the project name.
Write-Host "Generating plugin signing certificate..."
$pfxPath = "plugincert.pfx"
$certPassword = "templatecertpassword"

$rsaCert = [System.Security.Cryptography.RSA]::Create(2048)
$subject = [System.Security.Cryptography.X509Certificates.X500DistinguishedName]::new("CN=xrmbedrock")
$req = [System.Security.Cryptography.X509Certificates.CertificateRequest]::new(
    $subject, $rsaCert,
    [System.Security.Cryptography.HashAlgorithmName]::SHA256,
    [System.Security.Cryptography.RSASignaturePadding]::Pkcs1)

$ekuOids = [System.Security.Cryptography.OidCollection]::new()
$ekuOids.Add([System.Security.Cryptography.Oid]::new("1.3.6.1.5.5.7.3.3")) | Out-Null
$eku = [System.Security.Cryptography.X509Certificates.X509EnhancedKeyUsageExtension]::new($ekuOids, $true)
$req.CertificateExtensions.Add($eku)

$ku = [System.Security.Cryptography.X509Certificates.X509KeyUsageExtension]::new(
    [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]::DigitalSignature, $true)
$req.CertificateExtensions.Add($ku)

$cert = $req.CreateSelfSigned(
    [System.DateTimeOffset]::UtcNow,
    [System.DateTimeOffset]::UtcNow.AddYears(100))
$pfxBytes = $cert.Export(
    [System.Security.Cryptography.X509Certificates.X509ContentType]::Pfx, $certPassword)
[System.IO.File]::WriteAllBytes($pfxPath, $pfxBytes)
$cert.Dispose()
$rsaCert.Dispose()
Write-Host "Generated $pfxPath"

Write-Host "Restoring .NET tools..."
dotnet tool restore

Write-Host "Installing npm dependencies..."
Push-Location "src/Dataverse/WebResources"
npm install
Pop-Location

Write-Host "Generating Dataverse context from dev environment..."
Write-Host "You will be prompted to authenticate with your Dataverse environment."
dotnet fsi src/Tools/Daxif/GenerateCSharpContext.fsx
dotnet fsi src/Tools/Daxif/GenerateTypeScriptContext.fsx

Write-Host "Creating initial git commit..."
git add -A
git commit -q -m "Initial project setup from XrmBedrock template"

Write-Host "Post-template setup complete."
