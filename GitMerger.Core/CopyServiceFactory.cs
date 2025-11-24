namespace GitMerger.Core
{
    public static class CopyServiceFactory
    {
        public static ICopyService CreateCopyService(string serviceType)
        {
            return serviceType.ToLower() switch
            {
                "robocopy" => new RobocopyService(),
                "systemio" => new SystemIOCopyService(),
                _ => throw new ArgumentException($"Unknown copy service type: {serviceType}. Valid options are: robocopy, systemio")
            };
        }
    }
}
