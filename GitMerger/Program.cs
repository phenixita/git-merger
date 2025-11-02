using GitMerger.Core;
using LibGit2Sharp;

namespace GitMerger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Cartella repo partenza:");
            var repoSourcePath = Console.ReadLine();

            Console.WriteLine("Cartella repo destinazione:");
            var repoTargetPath = Console.ReadLine();

            Console.WriteLine("Subdir:");
            var subdir = Console.ReadLine();

            if (string.IsNullOrEmpty(repoSourcePath) || string.IsNullOrEmpty(repoTargetPath) || string.IsNullOrEmpty(subdir))
            {
                Console.WriteLine("Errore: tutti i campi sono obbligatori");
                return;
            }

            // Handle target directory initialization
            if (!PrepareTargetDirectory(repoTargetPath))
            {
                Console.WriteLine("Operazione annullata dall'utente");
                return;
            }

            var signature = new Signature(new Identity("test", "test@mail.eu"), DateTimeOffset.Now);


            using (IRepository repoSource = new Repository(repoSourcePath))
            using (IRepository repoTarget = new Repository(repoTargetPath))
            {
                Core.GitMerger.CloneLastCommitOfAllBranches(repoSource, repoTarget,
                    new RobocopyService(), "master", signature, signature,
                    new System.IO.DirectoryInfo(Path.Combine(repoTargetPath, subdir)));
            }

            Console.WriteLine("Fine");

            Console.ReadLine();
        }

        /// <summary>
        /// Prepares the target directory for git operations.
        /// Handles three scenarios: non-existent, empty, and non-empty directories.
        /// </summary>
        /// <param name="targetPath">Path to the target directory</param>
        /// <returns>True if directory is ready for git operations, false if operation was cancelled</returns>
        static bool PrepareTargetDirectory(string targetPath)
        {
            // Scenario 1: Target directory does not exist
            if (!Directory.Exists(targetPath))
            {
                Console.WriteLine($"La directory di destinazione '{targetPath}' non esiste. Creazione in corso...");
                Directory.CreateDirectory(targetPath);
                Console.WriteLine("Directory creata.");
                InitializeGitRepository(targetPath);
                return true;
            }

            // Check if it's already a valid git repository
            if (IsValidGitRepository(targetPath))
            {
                Console.WriteLine("La directory di destinazione è già un repository git valido.");
                return true;
            }

            // Scenario 3: Target directory exists and is empty
            if (IsDirectoryEmpty(targetPath))
            {
                Console.WriteLine("La directory di destinazione è vuota. Inizializzazione come repository git...");
                InitializeGitRepository(targetPath);
                return true;
            }

            // Scenario 2: Target directory exists but is not empty and not a git repo
            Console.WriteLine($"La directory di destinazione '{targetPath}' esiste ed è non vuota.");
            Console.WriteLine("Vuoi cancellare il contenuto della directory, inizializzarla come repository git e procedere? (s/n):");
            var response = Console.ReadLine()?.Trim().ToLower();

            if (response == "s" || response == "si" || response == "y" || response == "yes")
            {
                Console.WriteLine("Cancellazione del contenuto della directory in corso...");
                ClearDirectory(targetPath);
                Console.WriteLine("Contenuto cancellato.");
                InitializeGitRepository(targetPath);
                return true;
            }

            Console.WriteLine("Operazione annullata.");
            return false;
        }

        /// <summary>
        /// Initializes a directory as a git repository and creates an initial commit.
        /// This is necessary because LibGit2Sharp requires at least one commit to work with branches.
        /// </summary>
        /// <param name="path">Path to initialize</param>
        static void InitializeGitRepository(string path)
        {
            Repository.Init(path);
            Console.WriteLine("Repository git inizializzato.");

            // Create an initial empty commit so the repository has a master branch
            using (var repo = new Repository(path))
            {
                var signature = new Signature(new Identity("git-merger", "git-merger@local"), DateTimeOffset.Now);
                
                // Create an empty .gitkeep file to have something to commit
                var gitkeepPath = Path.Combine(path, ".gitkeep");
                File.WriteAllText(gitkeepPath, "");
                
                Commands.Stage(repo, ".gitkeep");
                repo.Commit("Initial commit", signature, signature);
                Console.WriteLine("Commit iniziale creato.");
            }
        }

        /// <summary>
        /// Checks if a directory is a valid git repository.
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>True if the directory is a valid git repository</returns>
        static bool IsValidGitRepository(string path)
        {
            try
            {
                using (var repo = new Repository(path))
                {
                    return true;
                }
            }
            catch (RepositoryNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a directory is empty.
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>True if the directory is empty</returns>
        static bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        /// <summary>
        /// Clears all contents of a directory.
        /// </summary>
        /// <param name="path">Path to the directory to clear</param>
        static void ClearDirectory(string path)
        {
            var di = new DirectoryInfo(path);

            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (var dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}
