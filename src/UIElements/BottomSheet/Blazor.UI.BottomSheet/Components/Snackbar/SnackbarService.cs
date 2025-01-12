namespace Blazor.UI.BottomSheet.Components.Snackbar;

public class SnackbarService
{
    public event EventHandler<SnackbarShowEventArgs>? OnShow;
    
    public void Show(string message, bool closable = true, Action? action = null)
    {
        OnShow?.Invoke(this, new SnackbarShowEventArgs(message, closable, action));
    }
}

public sealed class SnackbarShowEventArgs(string message, bool closable = true, Action? action = null) : EventArgs
{
    public string Message { get; } = message;
    public bool Closable { get; } = closable;
    public Action? Action { get; } = action;
}
