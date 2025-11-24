using System.IO;

namespace GitMerger.Core
{
    public class SystemIOCopyService : ICopyService
    {
        public void Copy(DirectoryInfo source, DirectoryInfo target, bool exludeGitPrivateFolder)
        {
            if (!source.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory not found: {source.FullName}");
            }

            // Create target directory if it doesn't exist
            if (!target.Exists)
            {
                target.Create();
            }

            // Copy all files
            foreach (var file in source.GetFiles())
            {
                var targetFilePath = Path.Combine(target.FullName, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            // Recursively copy subdirectories
            foreach (var subDir in source.GetDirectories())
            {
                // Skip .git directory if specified
                if (exludeGitPrivateFolder && subDir.Name.Equals(".git", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var targetSubDir = new DirectoryInfo(Path.Combine(target.FullName, subDir.Name));
                Copy(subDir, targetSubDir, exludeGitPrivateFolder);
            }
        }
    }
}
