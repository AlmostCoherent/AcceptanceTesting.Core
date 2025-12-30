using NorthStandard.Testing.Hosting.Domain.Abstractions;
using NorthStandard.Testing.Hosting.Infrastructure.Angular;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;

namespace NorthStandard.Testing.Hosting.Infrastructure.Angular
{
    /// <summary>
    /// Manages the Angular development server lifecycle for acceptance tests.
    /// 
    /// Use this class to start the dev server locally during test setup, or point tests at an externally-running instance.
    /// In CI/pipeline scenarios, the Angular app can be started independently and the test runner simply connects to it.
    /// </summary>
    public class AngularServer(AngularTestingProfile profile) {
        private Process? _process;
        private readonly ManualResetEvent _ready = new(false);

    /// <summary>
    /// Starts the Angular development server (ng serve) for the given project.
    /// The server will listen on the specified port and remain in watch mode.
    /// </summary>
    /// <param name="projectDir">Root directory of the Angular project (containing angular.json).</param>
    /// <param name="port">Port to serve on. Defaults to 4200 (Angular default).</param>
    /// <remarks>
    /// This method blocks until the server reports successful compilation.
    /// Throws if startup takes longer than ~120 seconds.
    /// </remarks>
    public void StartDevServer(string projectDir, int port = 4200) {
        if (IsAlreadyRunning()) {
            Console.WriteLine("Angular dev server already running.");
            return;
        }

        var npxPath = FindNpxExe();
        var processStartInfo = new ProcessStartInfo {
            FileName = npxPath,
            Arguments = string.Format(profile.ApplicationRunCommand, port),
            WorkingDirectory = projectDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        InjectNodeFolderToPath(processStartInfo);

        _process = Process.Start(processStartInfo) ?? throw new InvalidOperationException("Failed to start Angular dev server.");

        AttachOutputListeners(port);

        if (!_ready.WaitOne(TimeSpan.FromSeconds(240)))
            throw new TimeoutException("Angular dev server did not start in time.");

        Console.WriteLine($"Angular dev server is running at http://localhost:{port}/");
    }

    /// <summary>
    /// Stops the running Angular dev server, if any.
    /// Safe to call even if no server is running.
    /// </summary>
    public void Stop() {
        if (_process == null || _process.HasExited)
            return;

        try {
            Console.WriteLine("Stopping Angular dev server...");
            _process.Kill(true);
            _process.Dispose();
            _process = null;
        }
        catch (Exception ex) {
            Console.Error.WriteLine($"Error stopping Angular dev server: {ex.Message}");
        }
    }

    /// <summary>
    /// Waits for the Angular app to be ready and responding to HTTP requests at the given URL.
    /// Useful when connecting to an externally-running instance.
    /// </summary>
    /// <param name="baseUrl">The URL to probe (e.g., http://localhost:4200/).</param>
    /// <param name="timeoutSeconds">How long to wait before giving up.</param>
    /// <exception cref="TimeoutException">Thrown if the app does not respond in time.</exception>
    public void WaitForReady(string baseUrl, int timeoutSeconds = 30) {
        using var client = new HttpClient();
        var timeout = TimeSpan.FromSeconds(timeoutSeconds);
        var sw = Stopwatch.StartNew();

        while (sw.Elapsed < timeout) {
            try {
                var response = client.GetAsync(baseUrl).Result;
                if (response.IsSuccessStatusCode) {
                    Console.WriteLine("Angular app is responding.");
                    return;
                }
            }
            catch {
                // Not ready yet; retry after a brief pause.
            }
            Thread.Sleep(500);
        }

        throw new TimeoutException($"Angular app did not respond within {timeoutSeconds} seconds at {baseUrl}");
    }

    private bool IsAlreadyRunning() => _process != null && !_process.HasExited;

    private void AttachOutputListeners(int port) {
        _ready.Reset();

        _process!.OutputDataReceived += (_, args) => {
            if (args.Data == null) return;

            var line = StripAnsi(args.Data);
            Console.WriteLine(line);

            if (line.Contains("Compiled successfully", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("Local:", StringComparison.OrdinalIgnoreCase)) {
                _ready.Set();
            }
        };

        _process.ErrorDataReceived += (_, args) => {
            if (args.Data != null)
                Console.Error.WriteLine(args.Data);
        };

        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
    }

    private static string StripAnsi(string input)
        => Regex.Replace(input, @"\x1B\[[0-9;]*[mK]", "");

    private static string FindNpxExe() {
        // Check if npx is available on PATH.
        if (FindOnPath("npx") is string npx)
            return npx;

        // Check common installation locations.
        var candidates = new[]
        {
            @"C:\nvm4w\nodejs\npx.cmd",
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "nodejs", "npx.cmd"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "nodejs", "npx.cmd")
        };

        foreach (var candidate in candidates) {
            if (File.Exists(candidate))
                return candidate;
        }

        throw new FileNotFoundException(
            "Could not find 'npx'. Ensure Node.js is installed and added to PATH. " +
            "If using nvm-windows, ensure the symlink is active (nvm use <version>).");
    }

    private static string? FindOnPath(string executable) {
        var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        var pathExt = Environment.GetEnvironmentVariable("PATHEXT") ?? ".EXE;.CMD;.BAT";
        var dirs = pathEnv.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
        var exts = pathExt.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (var dir in dirs) {
            foreach (var ext in exts) {
                var candidate = Path.Combine(dir, executable + ext);
                if (File.Exists(candidate))
                    return candidate;
            }
        }

        return null;
    }

    private static void InjectNodeFolderToPath(ProcessStartInfo processStartInfo) {
        try {
            var nodeDir = GetNodeDirectory();
            if (string.IsNullOrWhiteSpace(nodeDir))
                return;

            var currentPath = processStartInfo.Environment.ContainsKey("PATH") ? 
                processStartInfo.Environment["PATH"] : 
                Environment.GetEnvironmentVariable("PATH") ?? string.Empty;

            var dirs = currentPath!.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (dirs.Any(d => string.Equals(d.TrimEnd('\\'), nodeDir.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase)))
                return; // Already in PATH.

            processStartInfo.Environment["PATH"] = nodeDir + Path.PathSeparator + currentPath;
        }
        catch {
            // Silently fail; rely on caller's PATH.
        }
    }

    private static string GetNodeDirectory() {
        // Look for node.exe on current PATH.
        if (FindOnPath("node") is string nodePath)
            return Path.GetDirectoryName(nodePath) ?? string.Empty;

        // Check nvm environment variables.
        var nvmSymlink = Environment.GetEnvironmentVariable("NVM_SYMLINK");
        if (!string.IsNullOrWhiteSpace(nvmSymlink) && Directory.Exists(nvmSymlink))
            return nvmSymlink;

        // Check common locations.
        var candidates = new[]
        {
            @"C:\nvm4w\nodejs",
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "nodejs"),
        };

        foreach (var candidate in candidates) {
            if (Directory.Exists(candidate) && File.Exists(Path.Combine(candidate, "node.exe")))
                return candidate;
        }

        return string.Empty;
    }
}
}
