using System.Threading.Tasks;
using RemovingTraditionalBranching;

namespace DemoWebApp.Repositories {
    internal class FakeFileRepository : IFilesRepository {
        public Task<bool> SaveAsync(string fileName, byte[] content) {
            return Task.FromResult(true);
        }
    }
}