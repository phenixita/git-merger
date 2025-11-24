using Microsoft.VisualStudio.TestTools.UnitTesting;
using GitMerger.Core;

namespace GitMerger.Core.Tests
{
    [TestClass]
    public class CopyServiceFactoryTests
    {
        [TestMethod]
        public void CreateCopyService_ShouldReturnRobocopyService_ForRobocopy()
        {
            // Act
            var service = CopyServiceFactory.CreateCopyService("robocopy");
            
            // Assert
            Assert.IsInstanceOfType(service, typeof(RobocopyService));
        }
        
        [TestMethod]
        public void CreateCopyService_ShouldReturnSystemIOCopyService_ForSystemIO()
        {
            // Act
            var service = CopyServiceFactory.CreateCopyService("systemio");
            
            // Assert
            Assert.IsInstanceOfType(service, typeof(SystemIOCopyService));
        }
        
        [TestMethod]
        public void CreateCopyService_ShouldBeCaseInsensitive()
        {
            // Act
            var service1 = CopyServiceFactory.CreateCopyService("ROBOCOPY");
            var service2 = CopyServiceFactory.CreateCopyService("SystemIO");
            
            // Assert
            Assert.IsInstanceOfType(service1, typeof(RobocopyService));
            Assert.IsInstanceOfType(service2, typeof(SystemIOCopyService));
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateCopyService_ShouldThrowException_ForInvalidType()
        {
            // Act
            CopyServiceFactory.CreateCopyService("invalid");
        }
    }
}
