namespace SD.Core.Shared.Models;

public class HeightSegment(double height, double thickness, int order)
{
    public double Height { get; private set; } = height;
    public double Thickness { get; private set; } = thickness;
    public int Order { get; private set; } = order;  
}
