namespace SD.Tests.Strand7.StepDefinitions;

[Binding]
public sealed class AxialBendingStepDefinitions(IUlsDesignResults ulsDesignResults)
{
    private readonly IUlsDesignResults _ulsDesignResults = ulsDesignResults ?? throw new ArgumentNullException(nameof(ulsDesignResults));

    private const double _accuracy = 0.01;

    [Then("the w one major should be {float} and w one minor should be {float} for beam {int}")]
    public void ThenTheWOneMajorShouldBeAndWOneMinorShouldBeForBeam(float w1Major, float w1Minor, int beamNumber)
    {
        var sansUlsResult = _ulsDesignResults!.SansUlsResults!.FirstOrDefault(sur => sur.Beam.Number == beamNumber);
        sansUlsResult!.Capacity.ω1Major.Should().BeApproximately((double)w1Major, _accuracy);
        sansUlsResult!.Capacity.ω1Minor.Should().BeApproximately((double)w1Minor, _accuracy);
    }
}