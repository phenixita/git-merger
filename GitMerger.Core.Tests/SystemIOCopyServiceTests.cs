using Microsoft.VisualStudio.TestTools.UnitTesting;
using GitMerger.Core;
using System.IO;

namespace GitMerger.Core.Tests
{
    [TestClass]
    public class SystemIOCopyServiceTests
    {
        private string _testDir = string.Empty;
        
        [TestInitialize]
        public void Setup()
        {
            _testDir = Path.Combine(Path.GetTempPath(), "GitMergerTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDir);
        }
        
        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_testDir))
            {
                try
                {
                    Directory.Delete(_testDir, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }
        
        [TestMethod]
        public void Copy_ShouldCopyAllFiles()
        {
            // Arrange
            var sourceDir = Path.Combine(_testDir, "source");
            var targetDir = Path.Combine(_testDir, "target");
            Directory.CreateDirectory(sourceDir);
            
            File.WriteAllText(Path.Combine(sourceDir, "file1.txt"), "content1");
            File.WriteAllText(Path.Combine(sourceDir, "file2.txt"), "content2");
            
            var copyService = new SystemIOCopyService();
            
            // Act
            copyService.Copy(new DirectoryInfo(sourceDir), new DirectoryInfo(targetDir), false);
            
            // Assert
            Assert.IsTrue(File.Exists(Path.Combine(targetDir, "file1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(targetDir, "file2.txt")));
            Assert.AreEqual("content1", File.ReadAllText(Path.Combine(targetDir, "file1.txt")));
            Assert.AreEqual("content2", File.ReadAllText(Path.Combine(targetDir, "file2.txt")));
        }
        
        [TestMethod]
        public void Copy_ShouldCopySubdirectories()
        {
            // Arrange
            var sourceDir = Path.Combine(_testDir, "source");
            var targetDir = Path.Combine(_testDir, "target");
            var sourceSubDir = Path.Combine(sourceDir, "subdir");
            Directory.CreateDirectory(sourceSubDir);
            
            File.WriteAllText(Path.Combine(sourceSubDir, "file.txt"), "content");
            
            var copyService = new SystemIOCopyService();
            
            // Act
            copyService.Copy(new DirectoryInfo(sourceDir), new DirectoryInfo(targetDir), false);
            
            // Assert
            Assert.IsTrue(File.Exists(Path.Combine(targetDir, "subdir", "file.txt")));
            Assert.AreEqual("content", File.ReadAllText(Path.Combine(targetDir, "subdir", "file.txt")));
        }
        
        [TestMethod]
        public void Copy_ShouldExcludeGitDirectory_WhenFlagIsSet()
        {
            // Arrange
            var sourceDir = Path.Combine(_testDir, "source");
            var targetDir = Path.Combine(_testDir, "target");
            var gitDir = Path.Combine(sourceDir, ".git");
            Directory.CreateDirectory(gitDir);
            
            File.WriteAllText(Path.Combine(gitDir, "config"), "git config");
            File.WriteAllText(Path.Combine(sourceDir, "file.txt"), "content");
            
            var copyService = new SystemIOCopyService();
            
            // Act
            copyService.Copy(new DirectoryInfo(sourceDir), new DirectoryInfo(targetDir), true);
            
            // Assert
            Assert.IsFalse(Directory.Exists(Path.Combine(targetDir, ".git")));
            Assert.IsTrue(File.Exists(Path.Combine(targetDir, "file.txt")));
        }
        
        [TestMethod]
        public void Copy_ShouldIncludeGitDirectory_WhenFlagIsNotSet()
        {
            // Arrange
            var sourceDir = Path.Combine(_testDir, "source");
            var targetDir = Path.Combine(_testDir, "target");
            var gitDir = Path.Combine(sourceDir, ".git");
            Directory.CreateDirectory(gitDir);
            
            File.WriteAllText(Path.Combine(gitDir, "config"), "git config");
            
            var copyService = new SystemIOCopyService();
            
            // Act
            copyService.Copy(new DirectoryInfo(sourceDir), new DirectoryInfo(targetDir), false);
            
            // Assert
            Assert.IsTrue(Directory.Exists(Path.Combine(targetDir, ".git")));
            Assert.IsTrue(File.Exists(Path.Combine(targetDir, ".git", "config")));
        }
    }
}
