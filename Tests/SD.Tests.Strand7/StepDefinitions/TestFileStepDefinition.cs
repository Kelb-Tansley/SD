using SD.Core.Shared.Enum;
using SD.Element.Design.Sans.Services;
using SD.Tests.Shared.Strand7;

namespace SD.Tests.Strand7.StepDefinitions;

[Binding]
public class TestFileStepDefinition(IConnectionService connectionService,
                                    IDesignModel designModel,
                                    IStrandApiService strandApiService,
                                    IFemModelDisplayService femModelDisplayService,
                                    IEffectiveLengthService effectiveLengthService,
                                    IFemModelParameters femModelParameters,
                                    SansDesignService sansDesignService,
                                    IFemModel femModel)
{
    private readonly IConnectionService _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
    private readonly IDesignModel _designModel = designModel ?? throw new ArgumentNullException(nameof(designModel));
    private readonly IFemModel _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
    private readonly IStrandApiService _strandApiService = strandApiService ?? throw new ArgumentNullException(nameof(strandApiService));
    private readonly IFemModelDisplayService _femModelDisplayService = femModelDisplayService ?? throw new ArgumentNullException(nameof(femModelDisplayService));
    private readonly IEffectiveLengthService _effectiveLengthService = effectiveLengthService ?? throw new ArgumentNullException(nameof(effectiveLengthService));
    private readonly IFemModelParameters _femModelParameters = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));
    private readonly SansDesignService _sansDesignService = sansDesignService ?? throw new ArgumentNullException(nameof(sansDesignService));

    private const int _modelId = 1;

    [BeforeStep]
    public void OnLoad()
    {
        if (!_connectionService.IsApiConnected)
            _connectionService.ConnectToStrand7Api();
    }

    [Given("the fem test file name is (.*)")]
    public void GivenTheFemTestFileNameIs(string fileName)
    {
        LocateStrand7TestModel.Initialize(fileName, _femModel, _designModel, out _);
    }

    [When("the uls analysis is run")]
    public async Task WhenTheUlsAnalysisIsRun()
    {
        _strandApiService.OpenFemFile(_modelId, _femModel.FileName);

        _femModelDisplayService.LoadFemModelProperties(_modelId, DesignCode.SANS, _femModel.FileName, true);

        var settings = new ModelDesignSettings();
        _effectiveLengthService.CalculateDesignLengths(_modelId, false, _femModelParameters, settings);

        _femModelParameters.LoadCaseCombinations.ToList().ForEach(lcc => lcc.Include = true);

        await _sansDesignService.RunUlsDesign(_modelId, _femModelParameters.Beams.ToList());
    }
}