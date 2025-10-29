using SD.Element.Design.Sans.Constants;
using SD.Element.Design.Sans.Models;

namespace SD.Element.Design.Sans.Engine;
public class SansService
{
    /// <summary>
    /// Resistance factor for structural steel.
    /// </summary>
    protected static double Φ => SansMaterialConstants.Φ;

    protected static SectionClassification GetAxialClass(Beam beam)
    {
        var sansResistance = beam.Resistance as SansBeamResistance ?? throw new ArgumentNullException(nameof(beam.Resistance));
        return sansResistance.CompressionClass;
    }
}
