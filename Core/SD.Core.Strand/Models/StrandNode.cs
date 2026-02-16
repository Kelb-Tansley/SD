using System.Text;

namespace SD.Core.Strand.Models;

public class StrandNode
{
    public int NodeNumber { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public bool Created { get; set; } = false;
}