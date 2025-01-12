using Microsoft.AspNetCore.Components;

namespace Blazor.UI.BottomSheet.Components.BottomSheet.Services;

public class BottomSheetService
{
    public event EventHandler<BottomSheetEventArgs>? OnChange;

    public void ShowSheet<T>(object? parameters, BottomSheetHeight height, bool isModal = true) where T : ComponentBase
    {
        RenderFragment content = builder =>
        {
            builder.OpenComponent<T>(0);
            if (parameters != null) builder.AddComponentParameter(0, "Model", parameters);
            builder.CloseComponent();
        };
        
        OnChange?.Invoke(this, new BottomSheetEventArgs(true, isModal, height.Height, content));
    }
    
    public void ShowSheet<T>(BottomSheetHeight height, bool isModal = true) where T : ComponentBase
    {
        ShowSheet<T>(null!, height, isModal);
    }
    
    public void ShowSheet(RenderFragment content, BottomSheetHeight height, bool isModal = true)
    {
        OnChange?.Invoke(this, new BottomSheetEventArgs(true, isModal, height.Height, content));
    }
    
    public void HideSheet()
    {
        OnChange?.Invoke(this, BottomSheetEventArgs.Hide);
    }
}