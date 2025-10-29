namespace SD.Core.Shared.Models;
public partial class LoadCase : Identity
{
    public LoadCase(string name, int number)
    {
        Number = number;
        Name = name;
    }
}