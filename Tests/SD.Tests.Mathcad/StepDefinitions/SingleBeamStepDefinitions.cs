using FluentAssertions;
using SD.Core.Shared.Enum;
using SD.Core.Shared.Models.Sans;
using SD.MathcadPrime.Interfaces;
using TechTalk.SpecFlow.Infrastructure;

namespace SD.Tests.Mathcad.Sans.StepDefinitions;

[Binding]
public sealed class SingleBeamStepDefinitions
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
    private readonly ISpecFlowOutputHelper _output;
    private readonly int _modelId = 1;
    private string _mathcadSheet;
    private SansCapacity _mathcadResults;
    private SansUlsResult? _sansUlsResult;

    public SingleBeamStepDefinitions(IConnectionService connectionService,
                                     SansDesignService sansDesignService,
                                     IDesignModel designModel,
                                     IFemModelParameters femModelParameters,
                                     IFemModelDisplayService femModelDisplayService,
                                     IFemModel femModel,
                                     IUlsDesignResults ulsDesignResults,
                                     IStrandApiService strandApiService,
                                     IEffectiveLengthService effectiveLengthService,
                                     ISansMathcadService mathcadService,
                                     ISpecFlowOutputHelper output)
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
        _output = output;
    }

    [BeforeStep]
    public void OnLoad()
    {
        if (!_connectionService.IsApiConnected)
            _connectionService.ConnectToStrand7Api();
    }

    [Given("the Strand7 single beam file name is (.*)")]
    public void GivenTheSingleBeamStrand7FileNameIs(string fileName)
    {
        var thisLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        ArgumentException.ThrowIfNullOrWhiteSpace(thisLocation);

        _femModel.FileName = Path.Combine(thisLocation, $@"TestFiles\Single\{fileName}");
        _designModel.IsLsa = true;
        _designModel.IsNla = false;
    }

    [When("the beam design is run")]
    public async Task WhenTheBeamDesignIsRun()
    {
        _strandApiService.OpenFemFile(_modelId, _femModel.FileName);

        _femModelDisplayService.LoadFemModelProperties(_modelId, DesignCode.SANS, _femModel.FileName, true);

        var settings = new BeamDesignSettings();
        _effectiveLengthService.CalculateDesignLengths(FemModels.ModelId, true, _femModelParameters, settings);

        _femModelParameters.LoadCaseCombinations.ToList().ForEach(lcc => lcc.Include = true);

        await _sansDesignService.RunUlsDesign(_modelId, _femModelParameters.Beams.ToList());
    }

    [When("the Mathcad test file is found for beam (.*)")]
    public void WhenTheMathcadTestFileIsFoundForBeam(int beamNumber)
    {
        ArgumentNullException.ThrowIfNull(_ulsDesignResults.SansUlsResults);
        _sansUlsResult = _ulsDesignResults.SansUlsResults.FirstOrDefault(sur => sur.Beam.Number == beamNumber);

        var thisLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        ArgumentException.ThrowIfNullOrWhiteSpace(thisLocation);

        ArgumentNullException.ThrowIfNull(_sansUlsResult?.Beam?.Section?.SectionType);
        _mathcadSheet = Path.Combine(thisLocation, $@"Templates\{_mathcadService.GetMathCadFileName(_sansUlsResult.Beam.Section.SectionType)}");
    }

    [When("the Mathcad file inputs are populated and run")]
    public void WhenTheMathcadFileInputsPopulatedAndRun()
    {
        ArgumentNullException.ThrowIfNull(_sansUlsResult);
        _mathcadService.ExportToMathcadFile(_mathcadSheet, _sansUlsResult);

        _mathcadResults = _mathcadService.ReadSansMathcadResults(_mathcadSheet);

        _mathcadService.SaveWorksheet();
        _mathcadService.CloseMathcad();
    }

    [Then("the beam resistance should match the mathcad output")]
    public void ThenTheBeamResistanceShouldMatchMatchcad()
    {
        var accuracy = 0.01;

        switch (_sansUlsResult?.DesignType)
        {
            case DesignType.Tension:
                {
                    _output.WriteLine("Tension Design Type");
                    _output.WriteLine("Tr");
                    ApproximateMatch(_sansUlsResult?.Capacity.Tr, _mathcadResults.Tr / 1000, accuracy);
                    _output.WriteLine("SlendernessMajor");
                    ApproximateMatch(_sansUlsResult?.Capacity.SlendernessMajor, _mathcadResults.SlendernessMajor, accuracy);
                    _output.WriteLine("SlendernessMinor");
                    ApproximateMatch(_sansUlsResult?.Capacity.SlendernessMinor, _mathcadResults.SlendernessMinor, accuracy);
                    break;
                }
            case DesignType.Compression:
                {
                    _output.WriteLine("Compression Design Type");
                    _output.WriteLine("CrMajor");
                    ApproximateMatch(_sansUlsResult?.Capacity.CrMajor, _mathcadResults.CrMajor / 1000, accuracy);
                    _output.WriteLine("CrMinor");
                    ApproximateMatch(_sansUlsResult?.Capacity.CrMinor, _mathcadResults.CrMinor / 1000, accuracy);
                    _output.WriteLine("SlendernessMajor");
                    ApproximateMatch(_sansUlsResult?.Capacity.SlendernessMajor, _mathcadResults.SlendernessMajor, accuracy);
                    _output.WriteLine("SlendernessMinor");
                    ApproximateMatch(_sansUlsResult?.Capacity.SlendernessMinor, _mathcadResults.SlendernessMinor, accuracy);
                    break;
                }
            case DesignType.Bending:
                {
                    _output.WriteLine("Bending Design Type");
                    _output.WriteLine("MrMinor");
                    ApproximateMatch(_sansUlsResult?.Capacity.MrMinor, _mathcadResults.MrMinor / 1000, accuracy);
                    _output.WriteLine("MrMajor");
                    ApproximateMatch(_sansUlsResult?.Capacity.MrMajor, _mathcadResults.MrMajor / 1000, accuracy);
                    _output.WriteLine("SlendernessMajor");
                    ApproximateMatch(_sansUlsResult?.Capacity.SlendernessMajor, _mathcadResults.SlendernessMajor, accuracy);
                    _output.WriteLine("SlendernessMinor");
                    ApproximateMatch(_sansUlsResult?.Capacity.SlendernessMinor, _mathcadResults.SlendernessMinor, accuracy);
                    _output.WriteLine("VrMinor");
                    ApproximateMatch(_sansUlsResult?.Capacity.VrMinor, _mathcadResults.VrMinor / 1000, accuracy);
                    _output.WriteLine("VrMajor");
                    ApproximateMatch(_sansUlsResult?.Capacity.VrMajor, _mathcadResults.VrMajor / 1000, accuracy);
                    break;
                }
            case DesignType.BendingAxial:
                {
                    _output.WriteLine("Bending Axial Design Type");
                    if (_sansUlsResult.Forces.MaxAxialForce > 0)
                    {
                        _output.WriteLine("Tr");
                        ApproximateMatch(_sansUlsResult?.Capacity.Tr, _mathcadResults.Tr / 1000, accuracy);
                    }
                    else if (_sansUlsResult.Forces.MinAxialForce < 0)
                    {
                        _output.WriteLine("CrMajor");
                        ApproximateMatch(_sansUlsResult?.Capacity.CrMajor, _mathcadResults.CrMajor / 1000, accuracy);
                        _output.WriteLine("CrMinor");
                        ApproximateMatch(_sansUlsResult?.Capacity.CrMinor, _mathcadResults.CrMinor / 1000, accuracy);
                    }

                    _output.WriteLine("MrMinor");
                    ApproximateMatch(_sansUlsResult?.Capacity.MrMinor, _mathcadResults.MrMinor / 1000, accuracy);
                    _output.WriteLine("MrMajor");
                    ApproximateMatch(_sansUlsResult?.Capacity.MrMajor, _mathcadResults.MrMajor / 1000, accuracy);
                    _output.WriteLine("SlendernessMajor");
                    ApproximateMatch(_sansUlsResult?.Capacity.SlendernessMajor, _mathcadResults.SlendernessMajor, accuracy);
                    _output.WriteLine("SlendernessMinor");
                    ApproximateMatch(_sansUlsResult?.Capacity.SlendernessMinor, _mathcadResults.SlendernessMinor, accuracy);
                    _output.WriteLine("VrMinor");
                    ApproximateMatch(_sansUlsResult?.Capacity.VrMinor, _mathcadResults.VrMinor / 1000, accuracy);
                    _output.WriteLine("VrMajor");
                    ApproximateMatch(_sansUlsResult?.Capacity.VrMajor, _mathcadResults.VrMajor / 1000, accuracy);
                    break;
                }
            default:
                throw new NotImplementedException(nameof(_sansUlsResult));
        }
    }

    private static void ApproximateMatch(double? value, double? match, double accuracy = 0.01D)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        if (match == null)
            throw new ArgumentNullException(nameof(match));

        ((double)value).Should().BeApproximately((double)match, (double)value * accuracy);
    }
}