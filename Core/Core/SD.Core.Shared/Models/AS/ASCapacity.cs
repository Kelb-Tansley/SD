namespace SD.Core.Shared.Models.AS;
public class ASCapacity : SectionCapacity
{
    public BendingConstants BendingConstants { get; set; }

    // === Combinations ===

    // Bending and Shear
    public double MajorBendingShear { get; set; }
    public double MinorBendingShear { get; set; }

    // Uniaxial Bending and Axial
    public double MajorSectionBendingTensionMrx { get; set; }
    public double MinorSectionBendingTensionMry { get; set; }
    public double MajorSectionBendingCompressionMrx { get; set; }
    public double MinorSectionBendingCompressionMry { get; set; }

    // Uniaxial Bending and Compression - In-plane Capacity
    public double MajorMemberBendingCompressionMix { get; set; }
    public double MinorMemberBendingCompressionMiy { get; set; }

    // Uniaxial Bending and Axial - Out-of-plane Capacity
    public double MajorMemberBendingCompressionMox { get; set; }
    public double MajorMemberBendingTensionMox { get; set; }    
}
