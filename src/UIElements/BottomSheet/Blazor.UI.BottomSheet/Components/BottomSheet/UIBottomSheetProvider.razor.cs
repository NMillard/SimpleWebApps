using Blazor.UI.BottomSheet.Components.BottomSheet.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Blazor.UI.BottomSheet.Components.BottomSheet;

public partial class UIBottomSheetProvider : ComponentBase, IAsyncDisposable
{
    [Inject] public BottomSheetService Service { get; set; } = null!;
    [Inject] public IJSRuntime Js { get; set; } = null!;

    private IJSObjectReference? module;
    private ElementReference? element;
    private RenderFragment? content;
    private bool show;
    private bool isModal;
    private double startY;
    private double moveDistance;
    private bool dragging;
    private double height;
    private double displayHeight;

    protected override Task OnInitializedAsync()
    {
        Service.OnChange += OnChange;
        return Task.CompletedTask;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            if (module != null)
            {
                height = await module.InvokeAsync<int>("getHeight", element);
            }
            return;
        }
        module = await Js.InvokeAsync<IJSObjectReference>("import", "./Components/BottomSheet/UIBottomSheetProvider.razor.js");
        
    }

    private void OnScrimClicked()
    {
        Service.HideSheet();
    }

    private void OnMouseDown(MouseEventArgs e)
    {
        Console.WriteLine("mouse down");
        dragging = true;
        moveDistance = 0;
        startY = e.ClientY;
    }

    private void OnMouseLeave(MouseEventArgs e)
    {
        dragging = false;
        startY = 0;
        moveDistance = 0;
    }

    private void OnMouseMove(MouseEventArgs e)
    {
        if (!dragging) return;
        if (moveDistance < 0) return;
        moveDistance = e.ClientY - startY;

        if (!(moveDistance > height / 2)) return;
        
        dragging = false;
        startY = 0;
        moveDistance = 0;
        Service.HideSheet();
    }

    private void OnMouseUp(MouseEventArgs e)
    {
        dragging = false;
    }

    private void OnChange(object? sender, BottomSheetEventArgs e)
    {
        show = e.Show;
        isModal = e.IsModal;
        content = e.Content;
        displayHeight = e.Height;
        
        StateHasChanged();
    }

    private string GetHeightCss()
    {
        return displayHeight switch {
            0.3 => "30%",
            0.5 => "50%",
            1 => "90%",
            _ => "30%"
        };
    }

    public async ValueTask DisposeAsync()
    {
        Service.OnChange -= OnChange;
        if (module != null) await module.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}