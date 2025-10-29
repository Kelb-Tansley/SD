using FluentAssertions;
using SD.Core.Shared.Enum;
using SD.Core.Shared.Models.Sans;
using SD.MathcadPrime.Interfaces;

namespace SD.Tests.Mathcad.Sans.StepDefinitions;
[Binding]
public sealed class CompressionOnlyStepDefinitions
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
    private readonly ISansMathcadService _mathcadService;
    private int _modelId = 1;
    private string _mathcadSheet;
    private SansCapacity _mathcadResults;
    private SansUlsResult? _sansUlsResult;

    public CompressionOnlyStepDefinitions(
        IConnectionService connectionService,
        SansDesignService sansDesignService,
        IDesignModel designModel,
        IFemModelParameters femModelParameters,
        IFemModelDisplayService femModelDisplayService,
        IFemModel femModel,
        IUlsDesignResults ulsDesignResults,
        IStrandApiService strandApiService,
        IEffectiveLengthService effectiveLengthService,
        ISansMathcadService mathcadService)
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
        _mathcadService = mathcadService ?? throw new ArgumentNullException(nameof(mathcadService));
    }

    [BeforeStep]
    public void OnLoad()
    {
        if (!_connectionService.IsApiConnected)
            _connectionService.ConnectToStrand7Api();
    }

    [Given("the compression test file name is (.*)")]
    public void GivenTheCompressionTestFileNameIs(string fileName)
    {
        var thisLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (thisLocation == null)
            throw new ArgumentNullException(nameof(thisLocation));

        _femModel.FileName = Path.Combine(thisLocation, $@"TestFiles\Compression\{fileName}");
        _designModel.IsLsa = true;
        _designModel.IsNla = false;
    }

    [Given("the mathcad compression test file name is (.*)")]
    public void GivenTheMathcadCompressionTestFileNameIs(string fileName)
    {
        var thisLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (thisLocation == null)
            throw new ArgumentNullException(nameof(thisLocation));

        _mathcadSheet = Path.Combine(thisLocation, $@"Templates\{fileName}");
    }

    [When("the compressive analysis is run")]
    public async Task WhenTheCompressiveAnalysisIsRun()
    {
        _strandApiService.OpenFemFile(_modelId, _femModel.FileName);

        _femModelDisplayService.LoadFemModelProperties(_modelId, DesignCode.SANS, _femModel.FileName, true);

        var settings = new BeamDesignSettings();
        _effectiveLengthService.CalculateDesignLengths(FemModels.ModelId, true, _femModelParameters, settings);

        _femModelParameters.LoadCaseCombinations.ToList().ForEach(lcc => lcc.Include = true);

        await _sansDesignService.RunUlsDesign(_modelId, _femModelParameters.Beams.ToList());
    }


    [When("the mathcad compression file inputs are populated and run for beam (.*)")]
    public async Task WhenTheMathcadCompressionPopulatedAndRun(int beamNumber)
    {
        _sansUlsResult = _ulsDesignResults.SansUlsResults.FirstOrDefault(sur => sur.Beam.Number == beamNumber);

        _mathcadService.ExportToMathcadFile(_mathcadSheet, _sansUlsResult);
        _mathcadResults = _mathcadService.ReadSansMathcadResults(_mathcadSheet);

        _mathcadService.CloseMathcad();
    }

    [Then("the compressive resistance should match the mathcad output")]
    public void ThenTheCompressiveResistanceShouldMatchMatchcad()
    {
        _sansUlsResult?.Capacity.Cr.Should().BeApproximately(_mathcadResults.Cr / 1000, 0.01);
    }
}