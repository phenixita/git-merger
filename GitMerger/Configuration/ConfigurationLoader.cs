using System.Text.Json;

namespace GitMerger.Configuration
{
    public class ConfigurationLoader
    {
        private const string DefaultConfigFileName = "gitmerger.json";

        public static GitMergerConfig LoadConfiguration(string? configFilePath = null)
        {
            var config = new GitMergerConfig();
            
            // Try to load from config file
            var configPath = configFilePath ?? DefaultConfigFileName;
            if (File.Exists(configPath))
            {
                try
                {
                    var json = File.ReadAllText(configPath);
                    var loadedConfig = JsonSerializer.Deserialize<GitMergerConfig>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    if (loadedConfig != null)
                    {
                        config = loadedConfig;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Failed to load configuration from {configPath}: {ex.Message}");
                }
            }
            
            return config;
        }

        public static void SaveExampleConfiguration(string filePath = "gitmerger.json")
        {
            var exampleConfig = new GitMergerConfig
            {
                SourceRepo = "/path/to/source",
                TargetRepo = "/path/to/target",
                Subdir = "src",
                Author = new AuthorConfig
                {
                    Name = "Your Name",
                    Email = "your@email.com"
                },
                RootBranch = "main",
                StagePatterns = new[] { "*" },
                CopyService = "systemio"  // Cross-platform default
            };

            var json = JsonSerializer.Serialize(exampleConfig, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            
            File.WriteAllText(filePath, json);
            Console.WriteLine($"Example configuration saved to {filePath}");
        }
    }
}
