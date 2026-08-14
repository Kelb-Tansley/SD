using SD.Core.Strand.Enum;

namespace SD.Tests.Strand7.StepDefinitions;

[Binding]
public sealed class AppliedLoadStepDefinitions(IUlsDesignResults ulsDesignResults)
{
    private readonly IUlsDesignResults _ulsDesignResults = ulsDesignResults ?? throw new ArgumentNullException(nameof(ulsDesignResults));

    private readonly double _accuracy = 0.0002;

    [Then("the (.*) result type: (.*) of beam (.*) should be (.*)")]
    public void ThenTheAppliedLoadShouldBe(string maxMin, BeamResultType resultType, int beamNumber, double value)
    {
        var sansUlsResult = _ulsDesignResults!.SansUlsResults!.FirstOrDefault(sur => sur.Beam.Number == beamNumber);

        switch (resultType)
        {
            case BeamResultType.ShearForceMinor:
                {
                    if (maxMin == "max")
                        sansUlsResult?.Forces.MaxVuMinor.Should().BeApproximately(value, _accuracy);
                    else
                        sansUlsResult?.Forces.MinVuMinor.Should().BeApproximately(value, _accuracy);
                }
                break;
            case BeamResultType.BendingMomentMinor:
                {
                    if (maxMin == "max")
                        sansUlsResult?.Forces.MaxMuMinor.Should().BeApproximately(value, _accuracy);
                    else
                        sansUlsResult?.Forces.MinMuMinor.Should().BeApproximately(value, _accuracy);
                }
                break;
            case BeamResultType.ShearForceMajor:
                {
                    if (maxMin == "max")
                        sansUlsResult?.Forces.MaxVuMajor.Should().BeApproximately(value, _accuracy);
                    else
                        sansUlsResult?.Forces.MinVuMajor.Should().BeApproximately(value, _accuracy);
                }
                break;
            case BeamResultType.BendingMomentMajor:
                {
                    if (maxMin == "max")
                        sansUlsResult?.Forces.MaxMuMajor.Should().BeApproximately(value, _accuracy);
                    else
                        sansUlsResult?.Forces.MinMuMajor.Should().BeApproximately(value, _accuracy);
                }
                break;
            case BeamResultType.AxialForce:
                {
                    if (maxMin == "max")
                        sansUlsResult?.Forces.MaxAxialForce.Should().BeApproximately(value, _accuracy);
                    else
                        sansUlsResult?.Forces.MinAxialForce.Should().BeApproximately(value, _accuracy);
                }
                break;
            default:
                break;
        }
    }
}