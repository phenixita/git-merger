using System.Diagnostics;
using System.IO;

namespace GitMerger.Core
{
    public class RobocopyService : ICopyService
    {

        public void Copy(DirectoryInfo source, DirectoryInfo target, bool exludeGitPrivateFolder)
        {
            string arguments = string.Format("{0} {1} /S /XD /COPY:DS {2}", source.FullName, target.FullName, exludeGitPrivateFolder ? ".git" : "");

            var copyProcess = Process.Start("robocopy", arguments);

            copyProcess.WaitForExit();
        }


    }
}
