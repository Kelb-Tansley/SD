namespace SD.Fem.Strand7.Interfaces;
public interface IEffectiveLengthService
{
    public void CalculateDesignLengths(int modelId, bool designLengthCalculated, IFemModelParameters femModelParameters, BeamDesignSettings designSettings);
}
