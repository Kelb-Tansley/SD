using SD.Core.Shared.Enum;
using SD.Tests.Shared.Strand7;

namespace SD.Tests.Strand7.StepDefinitions;

[Binding]
public sealed class ElementLengthStepDefinitions
{
    private readonly IConnectionService _connectionService;
    private readonly IDesignModel _designModel;
    private readonly IFemModel _femModel;
    private readonly IFemModelParameters _femModelParameters;
    private readonly IEffectiveLengthService _effectiveLengthService;
    private readonly IStrandApiService _strandApiService;
    private int _modelId = 1;

    public ElementLengthStepDefinitions(
        IConnectionService connectionService,
        IDesignModel designModel,
        IFemModel femModel,
        IFemModelParameters femModelParameters,
        IStrandApiService strandApiService,
        IEffectiveLengthService effectiveLengthService)
    {
        _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
        _designModel = designModel ?? throw new ArgumentNullException(nameof(designModel));
        _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
        _femModelParameters = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));
        _effectiveLengthService = effectiveLengthService ?? throw new ArgumentNullException(nameof(effectiveLengthService));
        _strandApiService = strandApiService ?? throw new ArgumentNullException(nameof(strandApiService));
    }

    [BeforeStep]
    public void OnLoad()
    {
        if (!_connectionService.IsApiConnected)
            _connectionService.ConnectToStrand7Api();
    }

    [Given("the fem test file name is (.*)")]
    public void GivenTheFemTestFileNameIs(string fileName)
    {
        LocateStrand7TestModel.Initialize(fileName, _femModel, _designModel, out _modelId);
    }

    [When("SANS ULS design is run")]
    public void WhenSansUlsIsRun()
    {
        _strandApiService.OpenFemFile(_modelId, _femModel.FileName);

        var resultsFileMock = new StrandResultFile();
        _strandApiService.GetFemModelParameters(_femModelParameters, DesignCode.SANS, _modelId, _designModel.IsLsa, resultsFileMock);

        var settings = new BeamDesignSettings();
        _effectiveLengthService.CalculateDesignLengths(_modelId, true, _femModelParameters, settings);
    }

    [Then("the L2 chain length of beam (.*) should be (.*)")]
    public void ThenTheL2ChainShouldBe(int beamNumber, double lengthL2)
    {
        var beam = _femModelParameters.Beams.FirstOrDefault(bm => bm.Number == beamNumber);
        beam.BeamChain.L2.Should().Be(lengthL2);
    }

    [Then("the L1 chain length of beam (.*) should be (.*)")]
    public void ThenTheL1ChainShouldBe(int beamNumber, double lengthL1)
    {
        var beam = _femModelParameters.Beams.FirstOrDefault(bm => bm.Number == beamNumber);
        beam.BeamChain.L1.Should().Be(lengthL1);
    }
}
