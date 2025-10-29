using SD.Core.Shared.Contracts;
using SD.Core.Shared.Enum;
using SD.Core.Shared.Models;
using SD.Element.Design.Interfaces;
using SD.Element.Design.Sans.Services;
using SD.Fem.Strand7.Interfaces;
using SD.Tests.Shared.Strand7;

namespace SD.Tests.Sans.StepDefinitions;

[Binding]
public sealed class DeflectionLimitStepDefinitions
{
    private readonly IDeflectionService _deflectionService;
    private readonly IConnectionService _connectionService;
    private readonly IDesignModel _designModel;
    private readonly IFemModelParameters _femModelParameters;
    private readonly IFemModelDisplayService _femModelDisplayService;
    private readonly IFemModel _femModel;
    private readonly IStrandApiService _strandApiService;

    private int _modelId;
    private List<DeflectionResult> _deflectionResults;
    private DeflectionAxis _deflectionAxis;
    private DeflectionMethod _deflectionMethod;
    public DeflectionLimitStepDefinitions(SansDeflectionService deflectionService,
                                          IConnectionService connectionService,
                                          IDesignModel designModel,
                                          IFemModel femModel,
                                          IFemModelParameters femModelParameters,
                                          IFemModelDisplayService femModelDisplayService,
                                          IStrandApiService strandApiService)
    {
        _deflectionService = deflectionService ?? throw new ArgumentNullException(nameof(deflectionService));
        _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
        _designModel = designModel ?? throw new ArgumentNullException(nameof(designModel));
        _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
        _femModelDisplayService = femModelDisplayService ?? throw new ArgumentNullException(nameof(femModelDisplayService));
        _femModelParameters = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));
        _strandApiService = strandApiService ?? throw new ArgumentNullException(nameof(strandApiService));
    }

    [BeforeStep]
    public void OnLoad()
    {
        if (!_connectionService.IsApiConnected)
            _connectionService.ConnectToStrand7Api();
    }

    [Given("the deflection test file name is (.*)")]
    public void GivenTheDeflectionTestFileNameIs(string fileName)
    {
        LocateStrand7TestModel.Initialize(fileName, _femModel, _designModel, out _modelId);
    }

    [Given("the deflection axis is (.*)")]
    public void GivenTheDeflectionAxisIs(DeflectionAxis deflectionAxis)
    {
        _deflectionAxis = deflectionAxis;
    }

    [Given("the deflection method is (.*)")]
    public void GivenTheDeflectionMethodIs(DeflectionMethod deflectionMethod)
    {
        _deflectionMethod = deflectionMethod;
    }

    [When("the deflection analysis is run")]
    public async Task WhenTheDeflectionAnalysisIsRun()
    {
        _strandApiService.OpenFemFile(_modelId, _femModel.FileName);

        _femModelDisplayService.LoadFemModelProperties(_modelId, DesignCode.SANS, _femModel.FileName, true);

        _femModelParameters.LoadCaseCombinations.ToList().ForEach(lcc => lcc.Include = true);

        _deflectionResults = await _deflectionService.GetDeflectionResults(_modelId, _femModelParameters.LoadCaseCombinations, _femModelParameters.Beams, _deflectionAxis, _deflectionMethod);
    }

    [Then("the span to deflection ratio of beam (.*) should be (.*)")]
    public void ThenTheSpanDeflectionRatioOfBeamShouldBe(int beamNumber, double spanDeflectionRatio)
    {
        var sansUlsResult = _deflectionResults.FirstOrDefault(res => res.BeamId == beamNumber);
        sansUlsResult.DeflectionRatio.Should().BeApproximately(spanDeflectionRatio, 0.01);
    }
}
