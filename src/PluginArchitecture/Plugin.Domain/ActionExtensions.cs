using System.Reflection;
using Plugin.Domain.Abstractions;

namespace Plugin.Domain;

public static class ActionExtensions {
    public static IEnumerable<string> GetActionNames(this IEnumerable<IAction> actions) => actions
        .Select(a => a.GetType().GetCustomAttribute<ActionMetaAttribute>()?.Name ?? a.GetType().Name);

    public static IAction? PickAction(this IEnumerable<IAction> actions, string actionName) => actions
        .SingleOrDefault(a => (a.GetType().GetCustomAttribute<ActionMetaAttribute>()?.Name ?? a.GetType().Name) == actionName);
}