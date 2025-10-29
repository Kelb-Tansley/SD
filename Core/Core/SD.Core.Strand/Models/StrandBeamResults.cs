using SD.Core.Shared.Models.BeamModels;

namespace SD.Core.Strand.Models;
public class StrandBeamResults
{
    /// <summary>
    /// The beam to which the sibling properties apply.
    /// </summary>
    public required Beam Beam { get; set; }

    /// <summary>
    /// The Strand7 load case combination Id.
    /// </summary>
    public required int LoadCaseId { get; set; }

    /// <summary>
    /// The number of stations along the beam element provided to Strand7. Strand7 returns a result array containing this amount of data points for each result type.
    /// </summary>
    public required int NumStations { get; set; }

    /// <summary>
    /// The number of columns of data returned by Strand7 for each station along the length of the beam.
    /// </summary>
    public required int NumColumns { get; set; }

    /// <summary>
    /// The beam result array returned by Strand7 for this beam.
    /// </summary>
    public required double[] BeamRes { get; set; }
    /// <summary>
    /// The beam stress results array returned by Strand7 for this beam.
    /// </summary>
    public required double[] BeamStressRes { get; set; }
    /// <summary>
    /// The beam quarter bending moment results array returned by Strand7 for this beam.
    /// </summary>
    public required double[] BeamQuarters { get; set; }
}
