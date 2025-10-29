namespace SD.Core.Shared.Models.AS;
public class ASUtilisation
{
    public double AxialTension { get; set; }
    public double AxialCompression { get; set; }

    public double MajorAxisBending { get; set; }
    public double MinorAxisBending { get; set; }

    public double MajorAxisShear { get; set; }
    public double MinorAxisShear { get; set; }

    public double MajorBendingShear { get; set; }
    public double MinorBendingShear { get; set; }
    public double BiaxialMemberBending { get; set; }
    public double BiaxialSectionBending { get; set; }

    public double MajorSectionBendingAxialMrx { get; set; }
    public double MinorSectionBendingAxialMry { get; set; }
    public double BiaxialSectionBendingAxial { get; set; }

    public double MajorMemberBendingCompressionMix { get; set; }
    public double MinorMemberBendingCompressionMiy { get; set; }

    public double MajorMemberBendingCompressionMox { get; set; }
    public double MajorMemberBendingTensionMox { get; set; }

    public double BiaxialMemberBendingCompression { get; set; }
    public double BiaxialMemberBendingTension { get; set; }

    public double AllowableStress { get; set; }

    // Combined Properties

    public double BiaxialBending => Math.Max(BiaxialMemberBending, BiaxialSectionBending);
    public double BiaxialBendingAxial
    {
        get
        {
            double[] utils = [
                BiaxialSectionBendingAxial,
                BiaxialMemberBendingCompression,
                BiaxialMemberBendingTension
            ];

            return utils.Max();
        }
    } 
    public double MajorBendingAxial
    {
        get
        {
            double[] utils = [
                MajorSectionBendingAxialMrx,
                MajorMemberBendingCompressionMix,
                MajorMemberBendingCompressionMox,
                MajorMemberBendingTensionMox
            ];

            return utils.Max();
        }
    }
    public double MinorBendingAxial => Math.Max(MinorSectionBendingAxialMry, MinorMemberBendingCompressionMiy);

    // Sumarries

    public string MaxUtilizationDescription => GetMaxDescription();
    public double MaxUtilizationPercentage => GetMaxOverall() * 100;
    public double MaxUtilization => GetMaxOverall();

    private double GetMaxOverall()
    {
        double[] allUtils = [
            AxialTension,
            AxialCompression,

            MajorAxisBending,
            MinorAxisBending,

            MajorAxisShear,
            MinorAxisShear,

            MajorBendingShear,
            MinorBendingShear,
            BiaxialMemberBending,
            BiaxialSectionBending,

            MajorSectionBendingAxialMrx,
            MinorSectionBendingAxialMry,
            BiaxialSectionBendingAxial,

            MajorMemberBendingCompressionMix,
            MinorMemberBendingCompressionMiy,

            MajorMemberBendingCompressionMox,
            MajorMemberBendingTensionMox,

            BiaxialMemberBendingCompression,
            BiaxialMemberBendingTension
        ];
        return allUtils.Max();
    }
    public string GetMaxDescription()
    {
        var max = GetMaxOverall();
        var failedReason = string.Empty;

        if (max == AxialTension) failedReason = "Axial Tension";
        else if (max == AxialCompression) failedReason = "Axial Compression";

        else if (max == MajorAxisBending) failedReason = "Major Axis Bending";
        else if (max == MinorAxisBending) failedReason = "Minor Axis Bending";

        else if (max == MajorAxisShear) failedReason = "Major Axis Shear";
        else if (max == MinorAxisShear) failedReason = "Minor Axis Shear";

        else if (max == MajorBendingShear) failedReason = "Combined Shear & Bending (Major Axis)";
        else if (max == MinorBendingShear) failedReason = "Combined Shear & Bending (Minor Axis)";
        else if (max == BiaxialMemberBending) failedReason = "Biaxial Member Bending";
        else if (max == BiaxialSectionBending) failedReason = "Biaxial Section Bending";

        else if (max == MajorSectionBendingAxialMrx) failedReason = "Major Section Bending & Axial (Mrx)";
        else if (max == MinorSectionBendingAxialMry) failedReason = "Minor Section Bending & Axial (Mry)";
        else if (max == BiaxialSectionBendingAxial) failedReason = "Biaxial Section Bending & Axial";

        else if (max == MajorMemberBendingCompressionMix) failedReason = "Major Member Bending & Compression (Mix)";
        else if (max == MinorMemberBendingCompressionMiy) failedReason = "Minor Member Bending & Compression (Miy)";

        else if (max == MajorMemberBendingCompressionMox) failedReason = "Major Member Bending & Compression (Mox)";
        else if (max == MajorMemberBendingTensionMox) failedReason = "Minor Member Bending & Tension (Mox)";

        else if (max == BiaxialMemberBendingCompression) failedReason = "Biaxial Member Bending & Compression";
        else if (max == BiaxialMemberBendingTension) failedReason = "Biaxial Member Bending & Tension";

        else if (max == AllowableStress) failedReason = "Von Mises";

        return failedReason;
    }
}
