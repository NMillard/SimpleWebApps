namespace Blazor.UI.BottomSheet.Components.BottomSheet.Services;

public class BottomSheetHeight
{
    private BottomSheetHeight(double height)
    {
        Height = height;
    }

    public double Height { get; }

    public static BottomSheetHeight QuarterHeight => new(0.3);
    public static BottomSheetHeight HalfHeight => new(0.5);
    public static BottomSheetHeight FullHeight => new(1);
}