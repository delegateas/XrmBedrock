// Post-template setup script
// Runs as a dotnet new post-action, or manually via: dotnet run --project Setup/PostSetup
//
// This is a .NET console app instead of a shell script because SNK and PFX generation
// require .NET crypto APIs — there is no cross-platform CLI equivalent (sn.exe is
// Windows-only, openssl is not standard on Windows).

using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

var projectRoot = FindProjectRoot();
Directory.SetCurrentDirectory(projectRoot);

Console.WriteLine("Initializing git repository...");
RunCommand("git", "init -q");

GenerateSnk();
GeneratePfx();
RunCommand("dotnet", "tool restore");
RunCommand("npm", "install", Path.Combine("src", "Dataverse", "WebResources"));

Console.WriteLine("Generating Dataverse context from dev environment...");
Console.WriteLine("You will be prompted to authenticate with your Dataverse environment.");
RunCommand("dotnet", "fsi src/Tools/Daxif/GenerateCSharpContext.fsx");
RunCommand("dotnet", "fsi src/Tools/Daxif/GenerateTypeScriptContext.fsx");

Console.WriteLine("Creating initial git commit...");
RunCommand("git", "add -A");
RunCommand("git", "commit -q -m \"Initial project setup from XrmBedrock template\"");

Console.WriteLine("Post-template setup complete.");

string FindProjectRoot()
{
    // Walk up from the working directory to find the .sln root
    var dir = Directory.GetCurrentDirectory();
    while (dir != null)
    {
        if (Directory.GetFiles(dir, "*.sln").Length > 0)
            return dir;
        dir = Directory.GetParent(dir)?.FullName;
    }

    // Fallback: assume current directory is the project root
    return Directory.GetCurrentDirectory();
}

void GenerateSnk()
{
    // Template engine replaces 'xrmbedrock' with the lowercase project name.
    Console.WriteLine("Generating strong name key...");
    var snkPath = "xrmbedrock.snk";

    using var rsa = RSA.Create(1024);
    var p = rsa.ExportParameters(true);

    using var ms = new MemoryStream();
    using var bw = new BinaryWriter(ms);

    // BLOBHEADER (8 bytes): type, version, reserved, algorithm
    bw.Write((byte)0x07);           // PRIVATEKEYBLOB
    bw.Write((byte)0x02);           // Version 2
    bw.Write((ushort)0);            // Reserved
    bw.Write((uint)0x00002400);     // CALG_RSA_SIGN

    // RSAPUBKEY (12 bytes): magic, bit length, public exponent
    bw.Write(new byte[] { 0x52, 0x53, 0x41, 0x32 });  // "RSA2" magic
    bw.Write((uint)(p.Modulus!.Length * 8));
    uint pubExp = 0;
    for (int i = 0; i < p.Exponent!.Length; i++)
    {
        pubExp |= (uint)p.Exponent[p.Exponent.Length - 1 - i] << (i * 8);
    }

    bw.Write(pubExp);

    // Key components: convert .NET big-endian arrays to CSP little-endian format
    foreach (var component in new[] { p.Modulus, p.P, p.Q, p.DP, p.DQ, p.InverseQ, p.D })
    {
        var le = (byte[])component!.Clone();
        Array.Reverse(le);
        bw.Write(le);
    }

    bw.Flush();
    File.WriteAllBytes(snkPath, ms.ToArray());
    Console.WriteLine($"Generated {snkPath}");
}

void GeneratePfx()
{
    // Template engine replaces 'templatecertpassword' with the cert password and 'xrmbedrock' with the project name.
    Console.WriteLine("Generating plugin signing certificate...");
    var pfxPath = "plugincert.pfx";
    var certPassword = "templatecertpassword";

    using var rsaCert = RSA.Create(2048);
    var subject = new X500DistinguishedName("CN=xrmbedrock");
    var req = new CertificateRequest(subject, rsaCert, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

    var ekuOids = new OidCollection { new("1.3.6.1.5.5.7.3.3") };
    req.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(ekuOids, true));
    req.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));

    using var cert = req.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(100));
    var pfxBytes = cert.Export(X509ContentType.Pfx, certPassword);
    File.WriteAllBytes(pfxPath, pfxBytes);
    Console.WriteLine($"Generated {pfxPath}");
}

void RunCommand(string command, string arguments, string? workingDirectory = null)
{
    Console.WriteLine($"Running: {command} {arguments}");

    // On Windows, commands like npm are .cmd files that need to run through cmd.exe
    var psi = new ProcessStartInfo
    {
        WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory(),
        UseShellExecute = false,
    };

    if (OperatingSystem.IsWindows())
    {
        psi.FileName = Environment.GetEnvironmentVariable("COMSPEC") ?? "cmd.exe";
        psi.Arguments = $"/c {command} {arguments}";
    }
    else
    {
        psi.FileName = command;
        psi.Arguments = arguments;
    }

    try
    {
        using var process = Process.Start(psi);
        if (process == null)
        {
            Console.WriteLine($"Warning: Failed to start '{command}'. You may need to run it manually.");
            return;
        }

        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            Console.WriteLine($"Warning: '{command} {arguments}' exited with code {process.ExitCode}");
        }
    }
    catch (System.ComponentModel.Win32Exception ex)
    {
        Console.WriteLine($"Warning: Failed to run '{command} {arguments}': {ex.Message}");
        Console.WriteLine($"You may need to run it manually.");
    }
}
