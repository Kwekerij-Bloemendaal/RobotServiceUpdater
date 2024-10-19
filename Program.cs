namespace RobotServiceUpdater
{
    using System;
    using System.Diagnostics;
    using LibGit2Sharp;
    using System.IO;

    class GitPullAndDotnetService
    {
        // Hardcoded Git repository URL en .NET commando
        private static readonly string repoUrl = "git@github.com:KwekerijBloemendaal/RobotService.git";
        private static readonly string dotnetCommand = "build";
        private static readonly string localRepoPath = Path.Combine("/home/raspberrypi/Documents/Repos/RobotService");

        static void Main(string[] args)
        {
            try
            {
                // Als de repo-map al bestaat, voer git pull uit
                if (Directory.Exists(localRepoPath))
                {
                    Console.WriteLine("Repository bestaat al. Voer git pull uit.");
                    using (var repo = new Repository(localRepoPath))
                    {
                        var remote = repo.Network.Remotes["origin"];
                        var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                        Commands.Fetch(repo, remote.Name, refSpecs, null, "");
                        var signature = new Signature("User", "email@example.com", DateTimeOffset.Now);
                        var mergeResult = Commands.Pull(repo, signature, null);
                        Console.WriteLine($"Merge result: {mergeResult.Status}");
                    }
                }
                else
                {
                    // Kloon de repository als deze nog niet bestaat
                    Console.WriteLine($"Kloon de repository van {repoUrl} naar {localRepoPath}.");
                    Repository.Clone(repoUrl, localRepoPath);
                }

                // Voer het dotnet-commando uit in de map van de gepulde repository
                //ExecuteDotnetCommand(dotnetCommand, localRepoPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout: {ex.Message}");
            }
        }

        // Methode voor het uitvoeren van een dotnet-commando
        static void ExecuteDotnetCommand(string command, string workingDirectory)
        {
            try
            {
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

                    Console.WriteLine("Output:");
                    Console.WriteLine(output);

                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine("Fout:");
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
