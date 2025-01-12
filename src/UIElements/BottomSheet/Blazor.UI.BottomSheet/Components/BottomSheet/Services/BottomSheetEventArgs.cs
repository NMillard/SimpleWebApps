using Microsoft.AspNetCore.Components;

namespace Blazor.UI.BottomSheet.Components.BottomSheet.Services;

public class BottomSheetEventArgs(bool show, bool isModal = true, double height = 0.5, RenderFragment? content = null)
    : EventArgs
{
    public static BottomSheetEventArgs Hide => new(false);

    public bool Show { get; } = show;
    public bool IsModal { get; } = isModal;
    public double Height { get; } = height;
    public RenderFragment? Content { get; } = content;
}