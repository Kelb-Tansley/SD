using FluentAssertions;
using SD.Core.Shared.Enum;
using SD.Core.Shared.Models.AS;
using SD.MathcadPrime.Interfaces;

namespace SD.Tests.Mathcad.AS.StepDefinitions;
[Binding]
public sealed class ShearStepDefinitions
{
    private readonly IConnectionService _connectionService;
    private readonly IElementDesignService _asDesignService;
    private readonly IDesignModel _designModel;
    private readonly IFemModelParameters _femModelParameters;
    private readonly IFemModel _femModel;
    private readonly IFemModelDisplayService _femModelDisplayService;
    private readonly IUlsDesignResults _ulsDesignResults;
    private readonly IStrandApiService _strandApiService;
    private readonly IEffectiveLengthService _effectiveLengthService;
    private readonly IAsMathcadService _mathcadService;
    private int _modelId = 1;
    private string _mathcadSheet;
    private SectionCapacity _mathcadResults;
    private ASUlsResult? _asUlsResult;

    public ShearStepDefinitions(
        IConnectionService connectionService,
        ASDesignService asDesignService,
        IDesignModel designModel,
        IFemModelParameters femModelParameters,
        IFemModelDisplayService femModelDisplayService,
        IFemModel femModel,
        IUlsDesignResults ulsDesignResults,
        IStrandApiService strandApiService,
        IEffectiveLengthService effectiveLengthService,
        IAsMathcadService mathcadService)
    {
        _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
        _asDesignService = asDesignService ?? throw new ArgumentNullException(nameof(asDesignService));
        _designModel = designModel ?? throw new ArgumentNullException(nameof(designModel));
        _femModelParameters = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));
        _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
        _femModelDisplayService = femModelDisplayService ?? throw new ArgumentNullException(nameof(femModelDisplayService));
        _ulsDesignResults = ulsDesignResults ?? throw new ArgumentNullException(nameof(ulsDesignResults));
        _strandApiService = strandApiService ?? throw new ArgumentNullException(nameof(strandApiService));
        _effectiveLengthService = effectiveLengthService ?? throw new ArgumentNullException(nameof(effectiveLengthService));
        _mathcadService = mathcadService ?? throw new ArgumentNullException(nameof(mathcadService));
    }

    [BeforeStep]
    public void OnLoad()
    {
        if (!_connectionService.IsApiConnected)
            _connectionService.ConnectToStrand7Api();
    }

    [Given("the shear test file name is (.*)")]
    public void GivenTheShearTestFileNameIs(string fileName)
    {
        var thisLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (thisLocation == null)
            throw new ArgumentNullException(nameof(thisLocation));

        _femModel.FileName = Path.Combine(thisLocation, $@"TestFiles\Shear\{fileName}");
        _designModel.SolverType = SolverType.LSA;
    }

    [Given("the mathcad shear test file name is (.*)")]
    public void GivenTheMathcadShearTestFileNameIs(string fileName)
    {
        var thisLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (thisLocation == null)
            throw new ArgumentNullException(nameof(thisLocation));

        _mathcadSheet = thisLocation + "\\" + $@"Templates\AS\{fileName}";
    }

    [When("the shear analysis is run")]
    public async Task WhenTheShearAnalysisIsRun()
    {
        _strandApiService.OpenFemFile(_modelId, _femModel.FileName);

        _femModelDisplayService.LoadFemModelProperties(_modelId, DesignCode.AS, _femModel.FileName, true);

        var settings = new BeamDesignSettings();
        _effectiveLengthService.CalculateDesignLengths(FemModels.ModelId, true, _femModelParameters, settings);

        _femModelParameters.LoadCaseCombinations.ToList().ForEach(lcc => lcc.Include = true);

        await _asDesignService.RunUlsDesign(_modelId, _femModelParameters.Beams?.ToList());
    }


    [When("the mathcad shear file inputs are populated and run for beam (.*)")]
    public async Task WhenTheMathcadShearPopulatedAndRun(int beamNumber)
    {
        _asUlsResult = _ulsDesignResults.AsUlsResults.FirstOrDefault(sur => sur.Beam.Number == beamNumber);
        _mathcadService.ExportToMathcadFile(_mathcadSheet, _asUlsResult);
        _mathcadResults = _mathcadService.ReadMathcadShearResults(_mathcadSheet);

        _mathcadService.CloseMathcad();
    }

    [Then("the shear resistance should match the mathcad output")]
    public void ThenTheShearResistanceShouldMatchMatchcad()
    {
        _asUlsResult?.Beam.Resistance.VrMajor.Should().BeApproximately(_mathcadResults.VrMajor, 0.01);
        //_asUlsResult?.Beam.Resistance.VrMinor.Should().BeApproximately(_mathcadResults.VrMinor, 0.01);
    }
}