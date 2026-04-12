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

var logPath = Path.Combine(projectRoot, "PostSetup.log");
using var logWriter = new StreamWriter(logPath, append: false) { AutoFlush = true };

Log($"Project root: {projectRoot}");
Log($"Log file: {logPath}");

Step(1, "Generating strong name key and plugin certificate");
GenerateSnk();
GeneratePfx();

Step(2, "Restoring dotnet tools");
RunCommand("dotnet", "tool restore");

Step(3, "Installing npm packages for WebResources");
RunCommand("npm", "install", Path.Combine("src", "Dataverse", "WebResources"));

Step(4, "Generating Dataverse C# context (requires browser login)");
Log("A browser window will open for OAuth authentication with your dev Dataverse environment.");
Log("Complete the login in the browser to continue.");
RunCommand("dotnet", "fsi src/Tools/Daxif/GenerateCSharpContext.fsx");

Step(5, "Generating Dataverse TypeScript context (requires browser login)");
Log("A second browser window will open for OAuth authentication.");
Log("Complete the login in the browser to continue.");
RunCommand("dotnet", "fsi src/Tools/Daxif/GenerateTypeScriptContext.fsx");

Step(6, "Initializing git repository and creating initial commit");
RunCommand("git", "init");
RunCommand("git", "add -A");
RunCommand("git", "commit -m \"Initial project setup from XrmBedrock template\"");

Log();
Log("Setup complete.");

void Log(string message = "")
{
    var line = message.Length > 0 ? $"[PostSetup] {message}" : string.Empty;
    logWriter.WriteLine(line);
    try { Console.WriteLine(line); } catch { /* ignore if stdout is broken */ }
}

void Step(int n, string description)
{
    Log();
    Log($"--- Step {n}: {description} ---");
}

string FindProjectRoot()
{
    // Walk up from the working directory to find the .sln/.slnx root
    var dir = Directory.GetCurrentDirectory();
    while (dir != null)
    {
        if (Directory.GetFiles(dir, "*.sln").Length > 0 || Directory.GetFiles(dir, "*.slnx").Length > 0)
            return dir;
        dir = Directory.GetParent(dir)?.FullName;
    }

    // Fallback: assume current directory is the project root
    return Directory.GetCurrentDirectory();
}

void GenerateSnk()
{
    // Template engine replaces 'templatecompanyname' with the company name.
    Log("Generating strong name key...");
    var snkPath = "templatecompanyname.snk";

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
    Log($"Generated {snkPath}");
}

void GeneratePfx()
{
    // Template engine replaces 'templatecertpassword' with the cert password and 'xrmbedrock' with the project name.
    Log("Generating plugin signing certificate...");
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
    Log($"Generated {pfxPath}");
}

void RunCommand(string command, string arguments, string? workingDirectory = null)
{
    Log($"Running: {command} {arguments}");

    // On Windows, commands like npm are .cmd files that need to run through cmd.exe
    var psi = new ProcessStartInfo
    {
        WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory(),
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
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
            Log($"Warning: Failed to start '{command}'. You may need to run it manually.");
            return;
        }

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null)
            {
                logWriter.WriteLine(e.Data);
            }
        };
        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null)
            {
                logWriter.WriteLine(e.Data);
            }
        };
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            Log($"Warning: '{command} {arguments}' exited with code {process.ExitCode}");
        }
        else
        {
            Log("Done.");
        }
    }
    catch (System.ComponentModel.Win32Exception ex)
    {
        Log($"Warning: Failed to run '{command} {arguments}': {ex.Message}");
        Log("You may need to run it manually.");
    }
}
