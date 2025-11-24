using GitMerger.Configuration;
using GitMerger.Core;
using LibGit2Sharp;

namespace GitMerger
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Load configuration from file (if exists)
                var config = ConfigurationLoader.LoadConfiguration();
                
                // Override with command-line arguments
                config = CommandLineParser.ParseArguments(args, config);
                
                // Prompt for missing required values
                config = PromptForMissingValues(config);
                
                // Validate configuration
                if (!ValidateConfiguration(config))
                {
                    Console.WriteLine("Errore: configurazione non valida");
                    return;
                }
                
                // Create signature from config
                var signature = new Signature(
                    new Identity(config.Author.Name, config.Author.Email), 
                    DateTimeOffset.Now
                );
                
                // Create copy service based on config
                var copyService = CopyServiceFactory.CreateCopyService(config.CopyService);
                
                // Execute merge
                using (IRepository repoSource = new Repository(config.SourceRepo))
                using (IRepository repoTarget = new Repository(config.TargetRepo))
                {
                    var subDirTarget = string.IsNullOrEmpty(config.Subdir) 
                        ? null 
                        : new System.IO.DirectoryInfo(Path.Combine(config.TargetRepo, config.Subdir));
                        
                    Core.GitMerger.CloneLastCommitOfAllBranches(
                        repoSource, 
                        repoTarget,
                        copyService, 
                        config.RootBranch, 
                        signature, 
                        signature,
                        subDirTarget,
                        config.StagePatterns
                    );
                }
                
                Console.WriteLine("Fine");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore: {ex.Message}");
                Environment.Exit(1);
            }
            
            if (args.Length == 0)
            {
                Console.ReadLine();
            }
        }
        
        static GitMergerConfig PromptForMissingValues(GitMergerConfig config)
        {
            if (string.IsNullOrEmpty(config.SourceRepo))
            {
                Console.WriteLine("Cartella repo partenza:");
                config.SourceRepo = Console.ReadLine() ?? string.Empty;
            }
            
            if (string.IsNullOrEmpty(config.TargetRepo))
            {
                Console.WriteLine("Cartella repo destinazione:");
                config.TargetRepo = Console.ReadLine() ?? string.Empty;
            }
            
            if (string.IsNullOrEmpty(config.Subdir))
            {
                Console.WriteLine("Subdir:");
                config.Subdir = Console.ReadLine() ?? string.Empty;
            }
            
            return config;
        }
        
        static bool ValidateConfiguration(GitMergerConfig config)
        {
            if (string.IsNullOrEmpty(config.SourceRepo) || 
                string.IsNullOrEmpty(config.TargetRepo) || 
                string.IsNullOrEmpty(config.Subdir))
            {
                Console.WriteLine("Errore: tutti i campi sono obbligatori");
                return false;
            }
            
            if (!Directory.Exists(config.SourceRepo))
            {
                Console.WriteLine($"Errore: directory sorgente non trovata: {config.SourceRepo}");
                return false;
            }
            
            if (!Directory.Exists(config.TargetRepo))
            {
                Console.WriteLine($"Errore: directory destinazione non trovata: {config.TargetRepo}");
                return false;
            }
            
            return true;
        }
    }
}

