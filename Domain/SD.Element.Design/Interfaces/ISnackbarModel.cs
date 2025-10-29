namespace SD.Element.Design.Interfaces;

public interface ISnackbarModel
{
    public string Message { get; set; }
    public bool IsActive { get; set; }
    public void ShowMessage(string message, int delay);
}