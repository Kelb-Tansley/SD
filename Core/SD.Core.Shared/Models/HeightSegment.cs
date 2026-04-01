namespace SD.Core.Shared.Models;

public class HeightSegment(double height, double thickness, int order)
{
    public double Height { get; set; } = height;
    public double Thickness { get; set; } = thickness;
    public int Order { get; set; } = order;  
}
