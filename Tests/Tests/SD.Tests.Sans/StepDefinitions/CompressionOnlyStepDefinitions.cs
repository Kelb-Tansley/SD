using SD.Core.Shared.Contracts;
using SD.Core.Shared.Enum;
using SD.Core.Shared.Models;
using SD.Element.Design.Interfaces;
using SD.Element.Design.Sans.Services;
using SD.Fem.Strand7.Interfaces;
using SD.Tests.Shared.Strand7;
using SD.UI.Constants;

namespace SD.Tests.Sans.StepDefinitions;

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
    private int _modelId = 1;

    public CompressionOnlyStepDefinitions(
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

    [Given("the compression test file name is (.*)")]
    public void GivenTheCompressionTestFileNameIs(string fileName)
    {
        LocateStrand7TestModel.Initialize(fileName, _femModel, _designModel, out _modelId);
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

    [Then("the compressive resistance of beam (.*) should be (.*)")]
    public void ThenTheCompressiveResistanceShouldBe(int beamNumber, double compressiveResistance)
    {
        var sansUlsResult = _ulsDesignResults.SansUlsResults.FirstOrDefault(sur => sur.Beam.Number == beamNumber);
        sansUlsResult?.Capacity.Cr.Should().Be(compressiveResistance);

    }
}
