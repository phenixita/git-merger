using LibGit2Sharp;

namespace GitMerger.Core
{
    public static class GitMerger
    {
        public static void CloneLastCommitOfAllBranches(IRepository repoSource,
            IRepository repoTarget,
            ICopyService copyService,
            string defaultBranchFriendlyName,
            Signature author,
            Signature committer,
            DirectoryInfo? subDirTarget = null)
        {
            foreach (var branch in repoSource.Branches)
            {
                CloneBranch(repoSource, repoTarget, copyService, branch, defaultBranchFriendlyName, author, committer, subDirTarget);
            }
        }

        public static void CloneBranch(IRepository repoSource,
            IRepository repoTarget,
            ICopyService copyService,
            Branch branchToClone,
            string rootBranchFriendlyName,
            Signature author,
            Signature committer,
            DirectoryInfo? subDirTarget = null
            )
        {
            if (rootBranchFriendlyName == null)
            {
                throw new ArgumentNullException(nameof(rootBranchFriendlyName));
            }

            Commands.Checkout(repoSource, branchToClone);

            Commands.Checkout(repoTarget, rootBranchFriendlyName);

            var aaaa = branchToClone.FriendlyName;
            if (aaaa.Contains("origin/")) aaaa = aaaa.Replace("origin/", "");

            if (!repoTarget.Branches.Any(b => b.FriendlyName == aaaa))
                repoTarget.CreateBranch(aaaa);

            Commands.Checkout(repoTarget, repoTarget.Branches[aaaa]);

            copyService.Copy(new DirectoryInfo(repoSource.Info.WorkingDirectory), subDirTarget ?? new DirectoryInfo(repoTarget.Info.WorkingDirectory), true);

            Commands.Stage(repoTarget, "*");

            try
            {
                repoTarget.Commit("Import " + branchToClone, author, committer);
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            }
                
        }
    }
}
