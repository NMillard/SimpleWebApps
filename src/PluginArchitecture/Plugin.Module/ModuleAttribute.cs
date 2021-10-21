namespace Plugin.Module;

[AttributeUsage(AttributeTargets.Assembly)]
public class ModuleAttribute : Attribute {
    public ModuleAttribute(string name) {
        ArgumentNullException.ThrowIfNull(name);
        Name = name;
    }

    public string Name { get; }
}