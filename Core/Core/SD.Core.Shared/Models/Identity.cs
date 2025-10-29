using CommunityToolkit.Mvvm.ComponentModel;

namespace SD.Core.Shared.Models;
public partial class Identity : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public int Number { get; set; }
}