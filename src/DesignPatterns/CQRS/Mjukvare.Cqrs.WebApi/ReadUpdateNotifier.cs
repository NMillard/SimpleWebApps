public sealed class ReadUpdateNotifier
{
    public event EventHandler? CheckinAdded;

    public void NotifyCheckinAdded()
    {
        CheckinAdded?.Invoke(this, EventArgs.Empty);
    }
}