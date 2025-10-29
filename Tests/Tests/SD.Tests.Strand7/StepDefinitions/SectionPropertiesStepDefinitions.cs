using SD.Core.Shared.Enum;
using SD.Core.Shared.Models.BeamModels;
using SD.Element.Design.Sans.Services;
using SD.Fem.Strand7.Helpers;
using SD.Tests.Shared.Strand7;
using TechTalk.SpecFlow.Infrastructure;

namespace SD.Tests.Strand7.StepDefinitions;

[Binding]
public sealed class SectionPropertiesStepDefinitions
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
    private readonly ISpecFlowOutputHelper _output;

    private int _modelId = 1;
    private double _accuracy = 0;

    private Section _beamProps;

    public SectionPropertiesStepDefinitions(
        IConnectionService connectionService,
        SansDesignService sansDesignService,
        IDesignModel designModel,
        IFemModelParameters femModelParameters,
        IFemModelDisplayService femModelDisplayService,
        IFemModel femModel,
        IUlsDesignResults ulsDesignResults,
        IStrandApiService strandApiService,
        IEffectiveLengthService effectiveLengthService,
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
        _output = output;
    }

    [BeforeStep]
    public void OnLoad()
    {
        if (!_connectionService.IsApiConnected)
            _connectionService.ConnectToStrand7Api();
    }

    [Given("the section properties test file name is (.*)")]
    public void GivenTheSectionPropertiesTestFileNameIs(string fileName)
    {
        LocateStrand7TestModel.Initialize(fileName, _femModel, _designModel, out _modelId);
    }

    [When("the section properties for property number (.*) are loaded with accuracy (.*)%")]
    public void WhenTheSectionPropertiesOfPropertyNumberAreLoadedWithAccuracy(int propertyNumber, double accuracy)
    {
        _accuracy = accuracy / 100;
        _strandApiService.OpenFemFile(_modelId, _femModel.FileName);

        var unitFactor = DetermineUnitFactors.GetModelUnitFactors(_modelId);
        _beamProps = _strandApiService.GetFemBeamSections(_modelId, unitFactor, DesignCode.SANS).FirstOrDefault(prop => prop.Number == propertyNumber);

        _output.WriteLine("Section Type = " + _beamProps.SectionType.ToString());
        _output.WriteLine("B1 = " + _beamProps.B1.ToString());
        _output.WriteLine("B2 = " + _beamProps.B2.ToString());
        _output.WriteLine("D = " + _beamProps.D.ToString());
        _output.WriteLine("T1 = " + _beamProps.T1.ToString());
        _output.WriteLine("T2 = " + _beamProps.T2.ToString());
        _output.WriteLine("T3 = " + _beamProps.T3.ToString());
    }

    [Then("the section property (.*) should be (.*)")]
    public void ThenTheSectionPropertyShouldBe(string property, double value)
    {
        var section = _beamProps;
        ArgumentNullException.ThrowIfNull(section);

        _output.WriteLine("Accuracy = " + (value * _accuracy).ToString());
        if (property == "Agr")
        {
            _output.WriteLine("Agr = " + section.Agr.ToString());
            value.Should().BeApproximately(section.Agr / 1000, value * _accuracy);
        }
        if (property == "RMinor")
        {
            _output.WriteLine("RMinor = " + section.RMinor.ToString());
            value.Should().BeApproximately(section.RMinor, value * _accuracy);
        }
        if (property == "RMajor")
        {
            _output.WriteLine("RMajor = " + section.RMajor.ToString());
            value.Should().BeApproximately(section.RMajor, value * _accuracy);
        }
        if (property == "AMinor")
        {
            _output.WriteLine("AMinor = " + section.AMinor.ToString());
            value.Should().BeApproximately(section.AMinor, value * _accuracy);
        }
        if (property == "AMajor")
        {
            _output.WriteLine("AMajor = " + section.AMajor.ToString());
            value.Should().BeApproximately(section.AMajor, value * _accuracy);
        }
        if (property == "CeMinor")
        {
            _output.WriteLine("CeMinor = " + section.CeMinor.ToString());
            value.Should().BeApproximately(section.CeMinor, value * _accuracy);
        }
        if (property == "CeMajor")
        {
            _output.WriteLine("CeMajor = " + section.CeMajor.ToString());
            value.Should().BeApproximately(section.CeMajor, value * _accuracy);
        }
        if (property == "ZeMajor")
        {
            _output.WriteLine("ZeMajor = " + section.ZeMajor.ToString());
            value.Should().BeApproximately(section.ZeMajor / 1000, value * _accuracy);
        }
        if (property == "ZeMinor")
        {
            _output.WriteLine("ZeMinor = " + section.ZeMinor.ToString());
            value.Should().BeApproximately(section.ZeMinor / 1000, value * _accuracy);
        }
        if (property == "ZplMajor")
        {
            _output.WriteLine("ZplMajor = " + section.ZplMajor.ToString());
            value.Should().BeApproximately(section.ZplMajor / 1000, value * _accuracy);
        }
        if (property == "ZplMinor")
        {
            _output.WriteLine("ZplMinor = " + section.ZplMinor.ToString());
            value.Should().BeApproximately(section.ZplMinor / 1000, value * _accuracy);
        }
        if (property == "IMajor")
        {
            _output.WriteLine("IMajor = " + section.IMajor.ToString());
            value.Should().BeApproximately(section.IMajor / 1000000, value * _accuracy);
        }
        if (property == "IMinor")
        {
            _output.WriteLine("IMinor = " + section.IMinor.ToString());
            value.Should().BeApproximately(section.IMinor / 1000000, value * _accuracy);
        }
        if (property == "J")
        {
            _output.WriteLine("J = " + section.J.ToString());
            _output.WriteLine("Adjusted Accuracy = " + (value * _accuracy).ToString());
            if (section.SectionType == SectionType.CircularHollow || section.SectionType == SectionType.RectangularHollow)
                value.Should().BeApproximately(section.J / 1000000, value * _accuracy);
            else
                value.Should().BeApproximately(section.J / 1000, value * _accuracy);
        }
        if (property == "Cw")
        {
            _output.WriteLine("Cw = " + section.Cw.ToString());
            value.Should().BeApproximately(section.Cw / 1000000000, value * _accuracy);
        }
        if (property == "Cx")
        {
            _output.WriteLine("D = " + section.D.ToString());
            _output.WriteLine("CeMajor = " + section.CeMajor.ToString());
            value.Should().BeApproximately(section.D - section.CeMajor, value * _accuracy);
        }
    }
}
