namespace SD.Core.Strand.Models;
public class BeamConnection
{
    public bool PrincipalEBottomDisconnected { get; set; }
    public bool PrincipalETopDisconnected { get; set; }
    public bool PrincipalZDisconnected { get; set; }
    public bool Principal2Disconnected { get; set; }
    public bool Principal1Disconnected { get; set; }

    /// <summary>
    /// True if all the other principal directions are disconnected.
    /// </summary>
    public bool AllDisconnected { get; set; } = true;

    public void SetDisconnected()
    {
        AllDisconnected = Principal1Disconnected
                          && Principal2Disconnected
                          && PrincipalZDisconnected
                          && PrincipalETopDisconnected
                          && PrincipalEBottomDisconnected;
    }
}
