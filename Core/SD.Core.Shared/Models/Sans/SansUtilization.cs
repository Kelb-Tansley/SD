namespace SD.Core.Shared.Models.Sans;
public class SansUtilization
{
    public double Tension { get; set; }
    public double Compression { get; set; }
    public double BendingMajor { get; set; }
    public double BendingMinor { get; set; }
    public double BiAxialBending { get; set; }
    public double CompressionAndBendingSectionStrength { get; set; }
    public double CompressionAndBendingMemberStrength { get; set; }
    public double CompressionAndBendingBucklingStrength { get; set; }
    public double ShearMajor { get; set; }
    public double ShearMinor { get; set; }
    public double ShearAndBendingMajor { get; set; }
    public double ShearAndBendingMinor { get; set; }
    public double TensionAndBending { get; set; }
    public double AllowableStress { get; set; }
    public double SlendernessMajor { get; set; }
    public double SlendernessMinor { get; set; }

    public string MaxUtilizationDescription { get => GetMaxDescription(); }
    public double MaxUtilizationPercentage { get => GetMaxOverall() * 100; }
    public double MaxUtilization { get => GetMaxOverall(); }

    private double GetMaxOverall()
    {
        double[] allUtils = [
            Tension,
            Compression,
            BendingMajor,
            BendingMinor,
            BiAxialBending,
            CompressionAndBendingSectionStrength,
            CompressionAndBendingMemberStrength,
            CompressionAndBendingBucklingStrength,
            ShearMajor,
            ShearMinor,
            ShearAndBendingMajor,
            ShearAndBendingMinor,
            TensionAndBending,
            AllowableStress,
            SlendernessMajor,
            SlendernessMinor
        ];
        return allUtils.Max();
    }
    public string GetMaxDescription()
    {
        var max = GetMaxOverall();
        var failedReason = string.Empty;

        if (max == Tension) failedReason = "Axial tension";
        if (max == Compression) failedReason = "Axial compression";
        else if (max == BendingMajor) failedReason = "Major axis bending";
        else if (max == BendingMinor) failedReason = "Minor axis bending";
        else if (max == BiAxialBending) failedReason = "Bi-axial bending";
        else if (max == CompressionAndBendingSectionStrength) failedReason = "Axial compression and bending 13.8 a)";
        else if (max == CompressionAndBendingMemberStrength) failedReason = "Axial compression and bending 13.8 b)";
        else if (max == CompressionAndBendingBucklingStrength) failedReason = "Axial compression and bending 13.8 c)";
        else if (max == ShearMajor) failedReason = "Major axis shear";
        else if (max == ShearMinor) failedReason = "Minor axis shear";
        else if (max == ShearAndBendingMajor) failedReason = "Combined shear and bending (Major axis)";
        else if (max == ShearAndBendingMinor) failedReason = "Combined shear and bending (Minor axis)";
        else if (max == TensionAndBending) failedReason = "Axial tension and bending";
        else if (max == AllowableStress) failedReason = "Von Mises allowable stress";
        else if (max == SlendernessMajor) failedReason = "Slenderness ratio (Major axis)";
        else if (max == SlendernessMinor) failedReason = "Slenderness ratio (Minor axis)";

        return failedReason;
    }
}
