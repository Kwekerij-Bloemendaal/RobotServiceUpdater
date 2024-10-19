namespace RobotServiceUpdater
{
    using System;
    using System.Diagnostics;
    using System.IO;

    class GitPullAndDotnetService
    {
        // Hardcoded Git repository URL en .NET commando
        private static readonly string repoUrl = "git@github.com:KwekerijBloemendaal/RobotService.git";
        private static readonly string servicePath = "/home/raspberrypi/Documents/Services/RobotService";
        private static readonly string dotnetCommand = "publish --configuration Release --runtime linux-arm64 --self-contained RobotService.csproj -o " + servicePath;
        private static readonly string localRepoPath = Path.Combine("/home/raspberrypi/Documents/Repos/RobotService");

        static void Main(string[] args)
        {
            try
            {
                // Als de repo-map al bestaat, voer git pull uit via extern Git-commando
                if (Directory.Exists(localRepoPath))
                {
                    Console.WriteLine("Repository bestaat al. Voer git pull uit.");
                    RunGitCommand("pull", localRepoPath);
                }
                else
                {
                    // Kloon de repository via extern Git-commando
                    Console.WriteLine($"Kloon de repository van {repoUrl} naar {localRepoPath}.");
                    RunGitCommand($"clone {repoUrl} {localRepoPath}", Directory.GetCurrentDirectory());
                }

                // Voer het dotnet-commando uit in de map van de gepulde repository
                ExecuteDotnetCommand(dotnetCommand, localRepoPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout: {ex.Message}");
            }
        }

        // Methode om een Git-commando uit te voeren
        static void RunGitCommand(string arguments, string workingDirectory)
        {
            try
            {
                Console.WriteLine($"Voer het git-commando uit: git {arguments} in de map {workingDirectory}");
                var processInfo = new ProcessStartInfo("git", arguments)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory
                };

                using (var process = Process.Start(processInfo))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    Console.WriteLine("Git output:");
                    Console.WriteLine(output);

                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine("Git fout:");
                        Console.WriteLine(error);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij het uitvoeren van het git-commando: {ex.Message}");
            }
        }

        // Methode voor het uitvoeren van een dotnet-commando
        static void ExecuteDotnetCommand(string command, string workingDirectory)
        {
            try
            {
                // Controleer of de output directory bestaat, maak deze anders aan
                if (!Directory.Exists(servicePath))
                {
                    Console.WriteLine($"Maak de output directory aan: {servicePath}");
                    Directory.CreateDirectory(servicePath);
                }

                Console.WriteLine($"Voer het dotnet-commando uit: {command} in de map {workingDirectory}");
                var processInfo = new ProcessStartInfo("dotnet", command)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory
                };

                using (var process = Process.Start(processInfo))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    Console.WriteLine("Dotnet output:");
                    Console.WriteLine(output);

                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine("Dotnet fout:");
                        Console.WriteLine(error);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij het uitvoeren van het dotnet-commando: {ex.Message}");
            }
        }
    }
}