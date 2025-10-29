namespace SD.Core.Shared.Contracts;
public interface IViewManagementModel
{
    public bool IsDialogOpen { get; set; }
    public bool IsDrawerOpen { get; set; }
    public bool IsRightDrawerOpen { get; set; }
}
