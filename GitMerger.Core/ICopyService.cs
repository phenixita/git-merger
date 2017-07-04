using System.IO;

namespace GitMerger.Core
{
    public interface ICopyService
    {
        void Copy(DirectoryInfo source, DirectoryInfo target, bool exludeGitPrivateFolder);
         
    }
}
