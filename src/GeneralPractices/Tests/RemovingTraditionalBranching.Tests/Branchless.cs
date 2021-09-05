using System.Text;
using System.Threading.Tasks;
using RemovingTraditionalBranching.Branchless;
using Xunit;

namespace RemovingTraditionalBranching.Tests {
    public class UnitTest1 {
        [Fact]
        public async Task Test1() {
            var sut = new FileSaver();
            sut.RegisterSink(() => new FileSystemSink(new FileSystemSavingOptions()));

            await sut.StoreFile(new FileSystemContext(Encoding.UTF8.GetBytes("hello"), "yes", "myfolder"));
        }
    }
}