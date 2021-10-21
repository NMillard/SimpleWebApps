namespace Plugin.Domain;

[AttributeUsage(AttributeTargets.Class)]
public class ActionMetaAttribute : Attribute {
    public ActionMetaAttribute(string name) {
        Name = name;
    }

    public string Name { get; }
}