using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RemovingTraditionalBranching.Traditional;
using Xunit;

namespace RemovingTraditionalBranching.Tests {
    public class Traditional {

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task AwkwardWithAllDependenciesInstantiated(bool savedSuccessfully) {
            var mock = new Mock<IFilesRepository>();
            mock.Setup(repo => repo.SaveAsync(It.IsAny<string>(), It.IsAny<byte[]>()))
                .ReturnsAsync(savedSuccessfully).Verifiable();
            
            var sut = new FileSaver(mock.Object, new FileSystemSavingOptions(), Mock.Of<IBlobStorage>());
            //var sut2 = new FileSaver(mock.Object, null!);

            byte[] content = Array.Empty<byte>();
            bool result = await sut.StoreFile("test-file.txt", content, StorageOption.Database, folderName: null);
            
            Assert.Equal(savedSuccessfully, result);
            mock.Verify();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task AwkwardDatabaseMock(bool savedSuccessfully) {
            var mock = new Mock<IFilesRepository>();
            mock.Setup(repo => repo.SaveAsync(It.IsAny<string>(), It.IsAny<byte[]>()))
                .ReturnsAsync(savedSuccessfully).Verifiable();
            
            var sut = new FileSaver(mock.Object, savingOptions: null!, blobStorage: null!);

            byte[] content = Array.Empty<byte>();
            bool result = await sut.StoreFile("test-file.txt", content, StorageOption.Database);
            
            Assert.Equal(savedSuccessfully, result);
            mock.Verify();
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task AwkwardBlobStorageMock(bool savedSuccessfully) {
            var mock = new Mock<IBlobStorage>();
            mock.Setup(b => b.StoreAsync(It.IsAny<string>(), It.IsAny<byte[]>())).ReturnsAsync(true).Verifiable();
            var sut = new FileSaver(repository: null!, savingOptions: null!, mock.Object);

            byte[] content = Array.Empty<byte>();
            bool result = await sut.StoreFile("test-file.txt", content, StorageOption.BlobStorage);
            
            Assert.Equal(savedSuccessfully, result);
            mock.Verify();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task WithDIContainer(bool savedSuccessfully) {
            var mock = new Mock<IFilesRepository>();
            mock.Setup(repo => repo.SaveAsync(It.IsAny<string>(), It.IsAny<byte[]>()))
                .ReturnsAsync(savedSuccessfully).Verifiable();
            
            // As typically wired up in an C# ASP.NET Core application.
            IServiceCollection services = new ServiceCollection()
                // When asking for an IFileRepository then provide a mock instance.
                .AddScoped<IFilesRepository>(_ => mock.Object)
                .AddScoped<IBlobStorage>(_ => Mock.Of<IBlobStorage>())
                .AddSingleton<FileSystemSavingOptions>(_ => new FileSystemSavingOptions { Path = "some-path" })
                .AddScoped<FileSaver>();

            ServiceProvider provider = services.BuildServiceProvider();

            var sut = provider.GetRequiredService<FileSaver>();

            byte[] content = Array.Empty<byte>();
            bool result = await sut.StoreFile("test-file.txt", content, StorageOption.Database, folderName: null);
            
            Assert.Equal(savedSuccessfully, result);
            mock.Verify();
        }
        
        [Fact]
        public async Task PolymorphicUseDatabase() {
            var mock = new Mock<IFilesRepository>();
            mock.Setup(r => r.SaveAsync(It.IsAny<string>(), It.IsAny<byte[]>()))
                .ReturnsAsync(true);

            var sut = new PolymorphicFileSaver(mock.Object, null!, null!);

            bool result = await sut.StoreFile(new FileSystemContext("d", Array.Empty<byte>(), "folder"));
        }
    }
}