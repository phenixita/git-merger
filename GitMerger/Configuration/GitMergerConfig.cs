namespace GitMerger.Configuration
{
    public class GitMergerConfig
    {
        public string SourceRepo { get; set; } = string.Empty;
        public string TargetRepo { get; set; } = string.Empty;
        public string Subdir { get; set; } = string.Empty;
        public AuthorConfig Author { get; set; } = new AuthorConfig();
        public string RootBranch { get; set; } = "master";
        public string[] StagePatterns { get; set; } = new[] { "*" };
        public string CopyService { get; set; } = "robocopy";
    }

    public class AuthorConfig
    {
        public string Name { get; set; } = "test";
        public string Email { get; set; } = "test@mail.eu";
    }
}
