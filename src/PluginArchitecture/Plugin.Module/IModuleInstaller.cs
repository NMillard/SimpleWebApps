using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Module;

public interface IModuleInstaller {
    void ConfigureServices(IServiceCollection services);
}