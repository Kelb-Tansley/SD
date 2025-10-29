using SD.Core.Shared.Enum;

namespace SD.Core.Shared.Models.BeamModels;

/// <summary>
/// Strand7 returns at most a 3 element section. Element 1 refers to T1 and B1 as returns from the Strand7 API. For two element sections, Element 2 refers that the outcome of T2 and D typically. 
/// The 3 element section then have T3 which refers to the web thickness. 
/// 
/// (Common Note: Element 1 refers to the bottom flange for I or H sections, the flange for T sections, the top and bottom flange for channels and rectangular sections, 
/// the x local axis leg of angles and the shell of circular sections. Element 2 refers to the top flange of I or H sections, the web of channels, 
/// rectangular and T sections, the y local axis leg of angles. Element 3 refers to the web of I or H sections.)
/// </summary>
public class SectionClassification(ElementClass element1, ElementClass? element2 = null, ElementClass? element3 = null)
{
    public ElementClass Element1 { get; set; } = element1;
    public ElementClass? Element2 { get; set; } = element2;
    public ElementClass? Element3 { get; set; } = element3;
    public ElementClass Section => Element2 == null ? Element3 == null ? Element1 : (ElementClass)Math.Max((int)Element3, (int)Element1) : Element3 == null ? (ElementClass)Math.Max((int)Element2, (int)Element1) : (ElementClass)Math.Max(Math.Max((int)Element3, (int)Element2), (int)Element1);
    public int SectionClassNumber => (int)Section;
}
