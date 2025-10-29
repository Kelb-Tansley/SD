namespace SD.Element.Design.Sans.Models;
public class MomentResistance
{
    public double MrMajor => Math.Min(Math.Min(MrMajorBottomUnsupported, MrMajorTopUnsupported), MrMajorSupported);
    public double MrMajorTopUnsupported { get; set; }
    public double MrMajorBottomUnsupported { get; set; }
    public double MrMajorSupported { get; set; }

    public double MrMinor => Math.Min(Math.Min(MrMinorBottomUnsupported, MrMinorTopUnsupported), MrMinorSupported);
    public double MrMinorTopUnsupported { get; set; }
    public double MrMinorBottomUnsupported { get; set; }
    public double MrMinorSupported { get; set; }
}
