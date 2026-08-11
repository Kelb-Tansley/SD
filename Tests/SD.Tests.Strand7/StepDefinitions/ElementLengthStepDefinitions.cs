using SD.Core.Shared.Enum;

namespace SD.Tests.Strand7.StepDefinitions;

[Binding]
public sealed class ElementLengthStepDefinitions(IFemModel femModel,
                                                 IFemModelParameters femModelParameters,
                                                 IStrandApiService strandApiService,
                                                 IEffectiveLengthService effectiveLengthService)
{
    private readonly IFemModel _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
    private readonly IFemModelParameters _femModelParameters = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));
    private readonly IEffectiveLengthService _effectiveLengthService = effectiveLengthService ?? throw new ArgumentNullException(nameof(effectiveLengthService));
    private readonly IStrandApiService _strandApiService = strandApiService ?? throw new ArgumentNullException(nameof(strandApiService));

    private readonly int _modelId = 1;

    [When("SANS ULS design is run")]
    public void WhenSansUlsIsRun()
    {
        _strandApiService.OpenFemFile(_modelId, _femModel.FileName);

        var resultsFileMock = new StrandResultFile();
        _strandApiService.GetFemModelParameters(_femModelParameters, DesignCode.SANS, _modelId, SolverType.LSA, resultsFileMock);

        var settings = new ModelDesignSettings();
        _effectiveLengthService.CalculateDesignLengths(_modelId, true, _femModelParameters, settings);
    }

    [Then("the L2 chain length of beam (.*) should be (.*)")]
    public void ThenTheL2ChainShouldBe(int beamNumber, double lengthL2)
    {
        var beam = _femModelParameters.Beams.FirstOrDefault(bm => bm.Number == beamNumber);
        beam!.BeamChain.L2.Should().Be(lengthL2);
    }

    [Then("the L1 chain length of beam (.*) should be (.*)")]
    public void ThenTheL1ChainShouldBe(int beamNumber, double lengthL1)
    {
        var beam = _femModelParameters.Beams.FirstOrDefault(bm => bm.Number == beamNumber);
        beam!.BeamChain.L1.Should().Be(lengthL1);
    }
}