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
        public void CreateCopyService_ShouldReturnCopyService_ForAuto()
        {
            // Act
            var service = CopyServiceFactory.CreateCopyService("auto");
            
            // Assert - Should return either RobocopyService or SystemIOCopyService depending on OS
            Assert.IsTrue(service is RobocopyService || service is SystemIOCopyService);
        }
        
        [TestMethod]
        public void CreateCopyService_ShouldBeCaseInsensitive()
        {
            // Act
            var service1 = CopyServiceFactory.CreateCopyService("ROBOCOPY");
            var service2 = CopyServiceFactory.CreateCopyService("SystemIO");
            var service3 = CopyServiceFactory.CreateCopyService("AUTO");
            
            // Assert
            Assert.IsInstanceOfType(service1, typeof(RobocopyService));
            Assert.IsInstanceOfType(service2, typeof(SystemIOCopyService));
            Assert.IsTrue(service3 is RobocopyService || service3 is SystemIOCopyService);
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
