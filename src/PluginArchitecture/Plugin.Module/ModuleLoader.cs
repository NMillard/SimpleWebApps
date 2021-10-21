using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Module;

public static class ModuleLoader {
    public static void Load(IServiceCollection services) {
        Console.WriteLine("---searching for modules---");

        // We need to actively find and load assemblies because they'll only be lazy loaded by
        // the runtime, i.e. if types from an assembly isn't explicitly used, then it won't be loaded.
        string[] directory = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
        Assembly[] assemblies = directory
            .Select(Assembly.LoadFrom)
            .Where(assembly => assembly.GetCustomAttribute<ModuleAttribute>() is { }).ToArray();

        foreach (Assembly assembly in assemblies) {
            var module = assembly.GetCustomAttribute<ModuleAttribute>()!;
            Console.WriteLine($"Loading {module.Name}");

            TypeInfo? installer = assembly.DefinedTypes.SingleOrDefault(IsInstaller);
            if (installer is null) {
                Console.WriteLine($"No installer found in {assembly.GetName()}");
                continue;
            }

            var serviceInstaller = Activator.CreateInstance(installer) as IModuleInstaller;
            serviceInstaller?.ConfigureServices(services);
        }
        Console.WriteLine("---");
        
        bool IsInstaller(Type type) => typeof(IModuleInstaller).IsAssignableFrom(type) &&
                                       !type.IsInterface &&
                                       !type.IsAbstract;
    }
}