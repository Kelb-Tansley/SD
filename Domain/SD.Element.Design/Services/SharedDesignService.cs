using SD.Core.Shared.Enum;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.BeamModels;
using SD.Core.Strand.Models;
using SD.Core.Strand;

namespace SD.Element.Design.Services;
public static class SharedDesignService
{
    public static DesignType GetDesignType(Beam beam, BeamForces forces)
    {
        if (forces.MaxAxialForce > 0 && forces.HasBending()) //Bending and Tension
        {
            if (beam.Section.SectionType == SectionType.Angle)// || beam.Section.SectionType == SectionType.T)
                return DesignType.Tension;

            return DesignType.BendingAxial;
        }

        if (forces.MinAxialForce < 0 && forces.HasBending()) //Bending and Compression
        {
            if (beam.Section.SectionType == SectionType.Angle)// || beam.Section.SectionType == SectionType.T)
                return DesignType.Compression;

            return DesignType.BendingAxial;
        }

        if (forces.MaxAxialForce > 0 && !forces.HasBending()) //Tension Only
            return DesignType.Tension;

        if (forces.MinAxialForce < 0 && !forces.HasBending()) //Compression Only
            return DesignType.Compression;

        if (forces.MinAxialForce == 0 && forces.MaxAxialForce == 0 && forces.HasBending()) //Bending Only
        {
            if (beam.Section.SectionType == SectionType.Angle)// || beam.Section.SectionType == SectionType.T)
                return DesignType.NoDesign;

            return DesignType.Bending;
        }

        return DesignType.NoDesign; //No design needed
    }

    public static BendingConstants SetBeamStationParameters(UnitFactor unitFactor, StrandBeamResults results)
    {
        var quarterPosition = 0;
        var halfPosition = results.NumColumns;
        var threeQuarterPosition = 2 * results.NumColumns;

        return new BendingConstants()
        {
            MuMajorQuarter = results.BeamQuarters[quarterPosition + St7.ipBeamBM2] * unitFactor.Force * unitFactor.Length,
            MuMajorHalf = results.BeamQuarters[halfPosition + St7.ipBeamBM2] * unitFactor.Force * unitFactor.Length,
            MuMajorThreeQuarter = results.BeamQuarters[threeQuarterPosition + St7.ipBeamBM2] * unitFactor.Force * unitFactor.Length,
            MuMinorQuarter = results.BeamQuarters[quarterPosition + St7.ipBeamBM1] * unitFactor.Force * unitFactor.Length,
            MuMinorHalf = results.BeamQuarters[halfPosition + St7.ipBeamBM1] * unitFactor.Force * unitFactor.Length,
            MuMinorThreeQuarter = results.BeamQuarters[threeQuarterPosition + St7.ipBeamBM1] * unitFactor.Force * unitFactor.Length,
        };
    }
}
