namespace SD.Core.Strand.Enum;
public enum BeamResultType
{
    ShearForceMinor = St7.ipBeamSF1,
    BendingMomentMinor = St7.ipBeamBM1,
    ShearForceMajor = St7.ipBeamSF2,
    BendingMomentMajor = St7.ipBeamBM2,
    AxialForce = St7.ipBeamAxialF,
    VonMisses = St7.ipVonMisesStress
}
