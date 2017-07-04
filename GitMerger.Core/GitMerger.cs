using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            DirectoryInfo subDirTarget = null)
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
            string defaultBranchFriendlyName,
            Signature author,
            Signature committer,
            DirectoryInfo subDirTarget = null
            )
        {
            if (defaultBranchFriendlyName == null)
            {
                throw new ArgumentNullException(nameof(defaultBranchFriendlyName));
            }

            Commands.Checkout(repoSource, branchToClone);

            Commands.Checkout(repoTarget, defaultBranchFriendlyName);

            if (!repoTarget.Branches.Any(b => b.FriendlyName == branchToClone.FriendlyName))
                repoTarget.CreateBranch(branchToClone.FriendlyName);

            Commands.Checkout(repoTarget, repoTarget.Branches[branchToClone.FriendlyName]);

            copyService.Copy(new DirectoryInfo(repoSource.Info.WorkingDirectory), subDirTarget ?? new DirectoryInfo(repoTarget.Info.WorkingDirectory), true);

            Commands.Stage(repoTarget, "*");

            repoTarget.Commit("Import " + branchToClone, author, committer);
        }
    }
}
