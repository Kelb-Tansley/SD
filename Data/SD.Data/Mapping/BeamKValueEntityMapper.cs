using SD.Data.Entities;

namespace SD.Data.Mapping;

public static class BeamKValueEntityMapper
{
    public static BeamKValue MapToBeamKValue(this BeamKValueEntity beam)
    {
        return new BeamKValue
        {
            BeamNumber = beam.BeamNumber,
            K1 = beam.K1,
            K2 = beam.K2,
            Kz = beam.Kz,
            KeTop = beam.KeTop,
            KeBottom = beam.KeBottom
        };
    }

    public static BeamKValueEntity MapToBeamKValueEntity(this BeamKValue beam, Guid femFileStableId)
    {
        return new BeamKValueEntity
        {
            FemFileStableId = femFileStableId,
            BeamNumber = beam.BeamNumber,
            K1 = beam.K1,
            K2 = beam.K2,
            Kz = beam.Kz,
            KeTop = beam.KeTop,
            KeBottom = beam.KeBottom
        };
    }

    public static void UpdateProperties(this BeamKValueEntity entity, BeamKValue beam)
    {
        entity.K1 = beam.K1;
        entity.K2 = beam.K2;
        entity.Kz = beam.Kz;
        entity.KeTop = beam.KeTop;
        entity.KeBottom = beam.KeBottom;
        entity.ModifiedDate = DateTime.UtcNow;
    }
}
