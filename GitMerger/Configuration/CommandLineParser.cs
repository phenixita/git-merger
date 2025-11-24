namespace GitMerger.Configuration
{
    public class CommandLineParser
    {
        public static GitMergerConfig ParseArguments(string[] args, GitMergerConfig baseConfig)
        {
            // First pass: Check for --config flag and load the config file
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower() == "--config" && i + 1 < args.Length)
                {
                    var configPath = args[i + 1];
                    baseConfig = ConfigurationLoader.LoadConfiguration(configPath);
                    break;
                }
            }
            
            var config = new GitMergerConfig
            {
                SourceRepo = baseConfig.SourceRepo,
                TargetRepo = baseConfig.TargetRepo,
                Subdir = baseConfig.Subdir,
                Author = new AuthorConfig
                {
                    Name = baseConfig.Author.Name,
                    Email = baseConfig.Author.Email
                },
                RootBranch = baseConfig.RootBranch,
                StagePatterns = baseConfig.StagePatterns,
                CopyService = baseConfig.CopyService
            };

            // Second pass: Process all other arguments (they override config values)
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "--source":
                    case "-s":
                        if (i + 1 < args.Length)
                            config.SourceRepo = args[++i];
                        break;
                    case "--target":
                    case "-t":
                        if (i + 1 < args.Length)
                            config.TargetRepo = args[++i];
                        break;
                    case "--subdir":
                    case "-d":
                        if (i + 1 < args.Length)
                            config.Subdir = args[++i];
                        break;
                    case "--author-name":
                    case "-n":
                        if (i + 1 < args.Length)
                            config.Author.Name = args[++i];
                        break;
                    case "--author-email":
                    case "-e":
                        if (i + 1 < args.Length)
                            config.Author.Email = args[++i];
                        break;
                    case "--root-branch":
                    case "-b":
                        if (i + 1 < args.Length)
                            config.RootBranch = args[++i];
                        break;
                    case "--copy-service":
                    case "-c":
                        if (i + 1 < args.Length)
                            config.CopyService = args[++i];
                        break;
                    case "--config":
                        // Already handled in first pass, skip
                        i++;
                        break;
                    case "--init-config":
                        var outputPath = i + 1 < args.Length && !args[i + 1].StartsWith("-") 
                            ? args[++i] 
                            : "gitmerger.json";
                        ConfigurationLoader.SaveExampleConfiguration(outputPath);
                        Environment.Exit(0);
                        break;
                    case "--help":
                    case "-h":
                        PrintHelp();
                        Environment.Exit(0);
                        break;
                }
            }

            return config;
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Git Merger - Configuration-Driven Git Repository Merger");
            Console.WriteLine();
            Console.WriteLine("Usage: GitMerger [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --config <path>           Load configuration from JSON file");
            Console.WriteLine("  --init-config [path]      Create example configuration file (default: gitmerger.json)");
            Console.WriteLine("  -s, --source <path>       Source repository path");
            Console.WriteLine("  -t, --target <path>       Target repository path");
            Console.WriteLine("  -d, --subdir <path>       Subdirectory in target repository");
            Console.WriteLine("  -n, --author-name <name>  Author name for commits");
            Console.WriteLine("  -e, --author-email <email> Author email for commits");
            Console.WriteLine("  -b, --root-branch <name>  Root branch name (default: master)");
            Console.WriteLine("  -c, --copy-service <type> Copy service type: robocopy, systemio (default: robocopy)");
            Console.WriteLine("  -h, --help                Show this help message");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  GitMerger --config myconfig.json");
            Console.WriteLine("  GitMerger -s /path/to/source -t /path/to/target -d src");
            Console.WriteLine("  GitMerger --init-config");
            Console.WriteLine();
            Console.WriteLine("For interactive mode, run without arguments.");
        }
    }
}
