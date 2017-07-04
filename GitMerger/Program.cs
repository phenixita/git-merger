using GitMerger.Core;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
