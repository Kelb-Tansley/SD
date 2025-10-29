using SD.Core.Shared.Enum;
using SD.Core.Strand.Enum;
using SD.Element.Design.Sans.Services;
using SD.Tests.Shared.Strand7;

namespace SD.Tests.Strand7.StepDefinitions;

[Binding]
public sealed class AppliedLoadStepDefinitions
{
    private readonly IConnectionService _connectionService;
    private readonly IElementDesignService _sansDesignService;
    private readonly IDesignModel _designModel;
    private readonly IFemModelParameters _femModelParameters;
    private readonly IFemModel _femModel;
    private readonly IFemModelDisplayService _femModelDisplayService;
    private readonly IUlsDesignResults _ulsDesignResults;
    private readonly IStrandApiService _strandApiService;
    private readonly IEffectiveLengthService _effectiveLengthService;

    private int _modelId = 1;
    private double _accuracy = 0.00001;
    private bool _designLengthCalculated = false;

    public AppliedLoadStepDefinitions(
        IConnectionService connectionService,
        SansDesignService sansDesignService,
        IDesignModel designModel,
        IFemModelParameters femModelParameters,
        IFemModelDisplayService femModelDisplayService,
        IFemModel femModel,
        IUlsDesignResults ulsDesignResults,
        IStrandApiService strandApiService,
        IEffectiveLengthService effectiveLengthService)
    {
        _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
        _sansDesignService = sansDesignService ?? throw new ArgumentNullException(nameof(sansDesignService));
        _designModel = designModel ?? throw new ArgumentNullException(nameof(designModel));
        _femModelParameters = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));
        _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
        _femModelDisplayService = femModelDisplayService ?? throw new ArgumentNullException(nameof(femModelDisplayService));
        _ulsDesignResults = ulsDesignResults ?? throw new ArgumentNullException(nameof(ulsDesignResults));
        _strandApiService = strandApiService ?? throw new ArgumentNullException(nameof(strandApiService));
        _effectiveLengthService = effectiveLengthService ?? throw new ArgumentNullException(nameof(effectiveLengthService));
    }

    [BeforeStep]
    public void OnLoad()
    {
        if (!_connectionService.IsApiConnected)
            _connectionService.ConnectToStrand7Api();
    }

    [Given("the applied load test file name is (.*)")]
    public void GivenTheAppliedLoadTestFileNameIs(string fileName)
    {
        LocateStrand7TestModel.Initialize(fileName, _femModel, _designModel, out _modelId);
    }

    [When("the uls analysis is run")]
    public async Task WhenTheUlsAnalysisIsRun()
    {
        _strandApiService.OpenFemFile(_modelId, _femModel.FileName);

        _femModelDisplayService.LoadFemModelProperties(_modelId, DesignCode.SANS, _femModel.FileName, true);

        var settings = new BeamDesignSettings();
        _effectiveLengthService.CalculateDesignLengths(FemModels.ModelId, _designLengthCalculated, _femModelParameters, settings);

        _femModelParameters.LoadCaseCombinations.ToList().ForEach(lcc => lcc.Include = true);

        await _sansDesignService.RunUlsDesign(_modelId, _femModelParameters.Beams.ToList());
    }

    [Then("the (.*) result type: (.*) of beam (.*) should be (.*)")]
    public void ThenTheAppliedLoadShouldBe(string maxMin, BeamResultType resultType, int beamNumber, double value)
    {
        var sansUlsResult = _ulsDesignResults.SansUlsResults.FirstOrDefault(sur => sur.Beam.Number == beamNumber);

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
