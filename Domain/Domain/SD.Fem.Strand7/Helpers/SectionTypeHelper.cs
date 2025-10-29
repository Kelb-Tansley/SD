namespace SD.Fem.Strand7.Helpers;
public class SectionTypeHelper
{
    public static SectionType SectionTypeFromStrand(int sectionType)
    {
        return sectionType switch
        {
            // Sections I and H
            St7.bsISection => SectionType.IorH,
            // Sections PFC and TFC
            St7.bsLipChannel => SectionType.LipChannel,
            // Sections EA and UA
            St7.bsLSection => SectionType.Angle,
            // Section CHS
            St7.bsCircularHollow => SectionType.CircularHollow,
            // Sections SHS and RHS
            St7.bsSquareHollow => SectionType.RectangularHollow,
            // Section T
            St7.bsTSection => SectionType.T,
            // Section NoDesign
            _ => SectionType.Unknown,
        };
    }
}