using FluentAssertions;
using SD.Core.Shared.Enum;
using SD.Core.Shared.Models.AS;
using SD.MathcadPrime.Interfaces;

namespace SD.Tests.Mathcad.AS.StepDefinitions;
[Binding]
public sealed class CombinationStepDefinitions
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
    private ASCapacity _mathcadResults;
    private ASUlsResult? _asUlsResult;

    public CombinationStepDefinitions(
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

    [Given("the combination test file name is (.*)")]
    public void GivenTheCombinationTestFileNameIs(string fileName)
    {
        var thisLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (thisLocation == null)
            throw new ArgumentNullException(nameof(thisLocation));

        _femModel.FileName = Path.Combine(thisLocation, $@"TestFiles\Combination\{fileName}");
        _designModel.SolverType = SolverType.LSA;
    }

    [Given("the mathcad combination test file name is (.*)")]
    public void GivenTheMathcadCombinationTestFileNameIs(string fileName)
    {
        var thisLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (thisLocation == null)
            throw new ArgumentNullException(nameof(thisLocation));

        _mathcadSheet = thisLocation + "\\" + $@"Templates\AS\{fileName}";
    }

    [When("the combination analysis is run")]
    public async Task WhenTheCombinationAnalysisIsRun()
    {
        _strandApiService.OpenFemFile(_modelId, _femModel.FileName);

        _femModelDisplayService.LoadFemModelProperties(_modelId, DesignCode.AS, _femModel.FileName, true);

        var settings = new BeamDesignSettings();
        _effectiveLengthService.CalculateDesignLengths(FemModels.ModelId, true, _femModelParameters, settings);

        _femModelParameters.LoadCaseCombinations.ToList().ForEach(lcc => lcc.Include = true);

        await _asDesignService.RunUlsDesign(_modelId, _femModelParameters.Beams.ToList());
    }

    [When("the mathcad combination file inputs are populated and run for beam (.*)")]
    public async Task WhenTheMathcadCombinationPopulatedAndRun(int beamNumber)
    {
        _asUlsResult = _ulsDesignResults.AsUlsResults.FirstOrDefault(sur => sur.Beam.Number == beamNumber);
        _mathcadService.ExportToMathcadFile(_mathcadSheet, _asUlsResult);
        _mathcadResults = _mathcadService.ReadASMathcadCombinationResults(_mathcadSheet);

        _mathcadService.CloseMathcad();
    }

    [Then("the combination capacity should match the mathcad output")]
    public void ThenTheCombinationResistanceShouldMatchMatchcad()
    {
        _asUlsResult?.Capacity.Tr.Should().BeApproximately(_mathcadResults.Tr / 1000, 0.01);
        _asUlsResult?.Capacity.Cr.Should().BeApproximately(_mathcadResults.Cr / 1000, 0.01);
        _asUlsResult?.Capacity.MrMajor.Should().BeApproximately(_mathcadResults.MrMajor / 1000, 0.01);
        _asUlsResult?.Capacity.MrMinor.Should().BeApproximately(_mathcadResults.MrMinor / 1000, 0.01);
        _asUlsResult?.Capacity.MajorBendingShear.Should().BeApproximately(_mathcadResults.MajorBendingShear / 1000, 0.01);
        _asUlsResult?.Capacity.MinorBendingShear.Should().BeApproximately(_mathcadResults.MinorBendingShear / 1000, 0.01);
        _asUlsResult?.Capacity.MajorSectionBendingTensionMrx.Should().BeApproximately(_mathcadResults.MajorSectionBendingTensionMrx / 1000, 0.01);
        _asUlsResult?.Capacity.MinorSectionBendingTensionMry.Should().BeApproximately(_mathcadResults.MinorSectionBendingTensionMry / 1000, 0.01);
        _asUlsResult?.Capacity.MajorSectionBendingCompressionMrx.Should().BeApproximately(_mathcadResults.MajorSectionBendingCompressionMrx / 1000, 0.01);
        _asUlsResult?.Capacity.MinorSectionBendingCompressionMry.Should().BeApproximately(_mathcadResults.MinorSectionBendingCompressionMry / 1000, 0.01);
        _asUlsResult?.Capacity.MajorMemberBendingCompressionMix.Should().BeApproximately(_mathcadResults.MajorMemberBendingCompressionMix / 1000, 0.01);
        _asUlsResult?.Capacity.MinorMemberBendingCompressionMiy.Should().BeApproximately(_mathcadResults.MinorMemberBendingCompressionMiy / 1000, 0.01);
        _asUlsResult?.Capacity.MajorMemberBendingCompressionMox.Should().BeApproximately(_mathcadResults.MajorMemberBendingCompressionMox / 1000, 0.01);
        //_asUlsResult?.Capacity.MajorMemberBendingTensionMox.Should().BeApproximately(_mathcadResults.MajorMemberBendingTensionMox / 1000, 0.01); // TODO: Mathcad using compression to calculate tension (|axial|)
    }
}