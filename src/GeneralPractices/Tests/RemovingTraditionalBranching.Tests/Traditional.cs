using System;
using System.Text;
using Moq;
using RemovingTraditionalBranching.Traditional;
using Xunit;

namespace RemovingTraditionalBranching.Tests {
    public class Traditional {
        [Fact]
        public void testName() {
            var mock = new Mock<IFilesRepository>();
            mock.Setup(r => r.SaveAsync(It.IsAny<string>(), It.IsAny<byte[]>()))
                .ReturnsAsync(true);
            
            var sut = new FileSaver(mock.Object, new FileSystemSavingOptions());

            sut.StoreFile("hello", Encoding.UTF8.GetBytes("hello"), StorageOption.FileSystem, "myfolder");
        }
    }
}