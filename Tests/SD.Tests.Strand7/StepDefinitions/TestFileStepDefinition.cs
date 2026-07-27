using SD.Tests.Shared.Strand7;

namespace SD.Tests.Strand7.StepDefinitions;

[Binding]
public class TestFileStepDefinition(IConnectionService connectionService,
                                    IDesignModel designModel,
                                    IFemModel femModel)
{
    private readonly IConnectionService _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
    private readonly IDesignModel _designModel = designModel ?? throw new ArgumentNullException(nameof(designModel));
    private readonly IFemModel _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));

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
}