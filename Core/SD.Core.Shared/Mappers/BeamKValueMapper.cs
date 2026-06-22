using SD.Core.Shared.Models.BeamModels;

namespace SD.Core.Shared.Mappers;

public static class BeamKValueMapper
{
    public static BeamKValue? MapToBeamKValue(this Beam beam, int beamNumber)
    {
        if (beam is null)
            return null;

        return new BeamKValue
        {
            BeamNumber = beamNumber,
            K1 = beam.BeamChain.K1,
            K2 = beam.BeamChain.K2,
            Kz = beam.BeamChain.Kz,
            KeTop = beam.BeamChain.KeTop,
            KeBottom = beam.BeamChain.KeBottom
        };
    }
}
