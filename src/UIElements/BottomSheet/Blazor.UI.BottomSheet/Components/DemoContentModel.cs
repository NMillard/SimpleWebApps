using Microsoft.AspNetCore.Components;

namespace Blazor.UI.BottomSheet.Components;

public class DemoContentModel
{
    public string Name { get; set; } = "none";
    public EventCallback? OnClicked { get; set; }
}