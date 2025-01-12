using Microsoft.AspNetCore.Components;

namespace Blazor.UI.BottomSheet.Components.Snackbar;

public partial class UISnackbarProvider : ComponentBase
{
    [Inject] public SnackbarService SnackbarService { get; set; } = null!;
    
    private bool show;
    private bool closable = true;
    private string message = string.Empty;
    private Action? action;
    private System.Timers.Timer? timer;
    private Queue<SnackbarShowEventArgs> queue = [];
    private CancellationTokenSource cts = new();
    private bool isProcessing = false;

    protected override void OnInitialized()
    {
        SnackbarService.OnShow += QueueSnackbar;
    }

    private void QueueSnackbar(object? sender, SnackbarShowEventArgs e)
    {
        queue.Enqueue(e);
        Console.WriteLine("Event queued");

        if (cts.IsCancellationRequested)
        {
            Console.WriteLine("Creating new token source");
            cts.Dispose();
            cts = new CancellationTokenSource();
        }
        
        if (isProcessing) return;
        
        Console.WriteLine("Start processing queue");
        isProcessing = true;
        ProcessQueueAsync(cts.Token).ConfigureAwait(false);
    }

    private async Task ProcessQueueAsync(CancellationToken cancellationToken = default)
    {
        while (queue.TryDequeue(out SnackbarShowEventArgs? e) && !cancellationToken.IsCancellationRequested)
        {
            show = true;
            message = e.Message;
            action = e.Action;
            closable = e.Closable;
            
            StateHasChanged();

            Console.WriteLine("Waiting 5 seconds");
            await Task.Delay(5000, cancellationToken).ConfigureAwait(false);
            show = false;
            StateHasChanged();
        }
        
        isProcessing = false;
        await cts.CancelAsync();
        cts.Dispose();
    }

    private void OnCloseClicked()
    {
        show = false;
        StateHasChanged();
    }

    private void OnMouseEnter()
    {
    }

    private void OnMouseLeave()
    {
    }
}