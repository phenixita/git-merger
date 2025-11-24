using System.Runtime.InteropServices;

namespace GitMerger.Core
{
    public static class CopyServiceFactory
    {
        public static ICopyService CreateCopyService(string serviceType)
        {
            return serviceType.ToLower() switch
            {
                "auto" => CreateAutoCopyService(),
                "robocopy" => new RobocopyService(),
                "systemio" => new SystemIOCopyService(),
                _ => throw new ArgumentException($"Unknown copy service type: {serviceType}. Valid options are: auto, robocopy, systemio")
            };
        }

        private static ICopyService CreateAutoCopyService()
        {
            // Automatically detect OS and return appropriate copy service
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new RobocopyService();
            }
            
            return new SystemIOCopyService();
        }
    }
}
