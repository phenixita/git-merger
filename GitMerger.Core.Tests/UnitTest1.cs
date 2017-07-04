using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using LibGit2Sharp;
using System.IO;

namespace GitMerger.Core.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CheckOutAndCloneInRoot()
        {
            //Assemble. 
            string repoSourceRelativePath = @"..\..\TestAssets\repoSource";
            string repoTargetRelativePathOriginal = @"..\..\TestAssets\repoTarget_original";
            string repoTargetRelativePath = @"..\..\TestAssets\repoTarget";
            const string referenceBranch = "master";

            var copyService = new RobocopyService();

            var signature = new Signature(new Identity("test", "test@mail.eu"), DateTimeOffset.Now);

            var repoSource = new Repository(repoSourceRelativePath);
            Commands.Checkout(repoSource, referenceBranch);

            var directoryTarget = new System.IO.DirectoryInfo(repoTargetRelativePath);
            DirCleaner.DeleteDirectory(directoryTarget.FullName);

            copyService.Copy(new System.IO.DirectoryInfo(repoTargetRelativePathOriginal), directoryTarget, false);

            var repoTarget = new Repository(repoTargetRelativePath);
            Commands.Checkout(repoSource, referenceBranch);

            //Act.
            GitMerger.CloneBranch(repoSource, repoTarget, copyService, repoSource.Branches[referenceBranch], referenceBranch, signature, signature, null);

            //Assert.
            Assert.IsTrue(CompareDirs.Compare(repoSource.Info.Path, repoTarget.Info.Path));
        }

        [TestMethod]
        public void CheckOutAndCloneInSubdir()
        {
            //Assemble. 
            string repoSourceRelativePath = @"..\..\TestAssets\repoSource";
            string repoTargetRelativePathOriginal = @"..\..\TestAssets\repoTarget_original";
            string repoTargetRelativePath = @"C:\temp\repoTarget\";

            const string referenceBranch = "master";

            var copyService = new RobocopyService();

            var signature = new Signature(new Identity("test", "test@mail.eu"), DateTimeOffset.Now);

            var repoSource = new Repository(repoSourceRelativePath);
            Commands.Checkout(repoSource, referenceBranch);

            var directoryTarget = new System.IO.DirectoryInfo(repoTargetRelativePath);
            DirCleaner.DeleteDirectory(directoryTarget.FullName);

            copyService.Copy(new System.IO.DirectoryInfo(repoTargetRelativePathOriginal), directoryTarget, false);

            var repoTarget = new Repository(directoryTarget.FullName);
            Commands.Checkout(repoSource, referenceBranch);

            //Act.
            GitMerger.CloneBranch(repoSource, repoTarget, copyService, repoSource.Branches[referenceBranch],
                referenceBranch, signature, signature, new System.IO.DirectoryInfo(Path.Combine(repoTargetRelativePath, "subDir")));

            //Assert.
            Assert.IsTrue(CompareDirs.Compare(repoSource.Info.Path, repoTarget.Info.Path));
        }
    }

}
