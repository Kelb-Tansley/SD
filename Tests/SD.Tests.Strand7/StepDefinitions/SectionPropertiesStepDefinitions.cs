using SD.Core.Shared.Enum;
using SD.Core.Shared.Models.BeamModels;
using SD.Element.Design.Sans.Services;
using SD.Fem.Strand7.Helpers;
using SD.Tests.Strand7.Helpers;

namespace SD.Tests.Strand7.StepDefinitions;

[Binding]
public sealed class SectionPropertiesStepDefinitions(
    IFemModel femModel,
    IStrandApiService strandApiService)
{
    private readonly IFemModel _femModel = femModel ?? throw new ArgumentNullException(nameof(femModel));
    private readonly IStrandApiService _strandApiService = strandApiService ?? throw new ArgumentNullException(nameof(strandApiService));

    private readonly int _modelId = 1;
    private double _accuracy = 0;

    private Section? _beamProps1;
    private Section? _beamProps2;

    [When("the section properties for property number (.*) and (.*) are loaded with accuracy (.*)%")]
    public void WhenTheSectionPropertiesOfPropertyNumberAreLoadedWithAccuracy(int propertyNumber1, int propertyNumber2, double accuracy)
    {
        _accuracy = accuracy / 100;
        _strandApiService.OpenFemFile(_modelId, _femModel.FileName);

        var unitFactor = DetermineUnitFactors.GetModelUnitFactors(_modelId);
        var (designableProperties, _) = _strandApiService.GetFemBeamSections(_modelId, unitFactor, DesignCode.SANS);
        _beamProps1 = designableProperties.FirstOrDefault(prop => prop.Number == propertyNumber1)!;
        _beamProps2 = designableProperties.FirstOrDefault(prop => prop.Number == propertyNumber2)!;
    }

    [Then("the section property (.*) should be (.*)")]
    public void ThenTheSectionPropertyShouldBe(string property, double value)
    {
        AssertSectionProperties(property, value, _beamProps1!);
        AssertSectionProperties(property, value, _beamProps2!);
        SectionAssertions.AssertSectionsAreEqual(_beamProps1!, _beamProps2!, _accuracy);
    }

    private void AssertSectionProperties(string property, double value, Section section)
    {
        ArgumentNullException.ThrowIfNull(section);

        Console.WriteLine("Accuracy = " + (value * _accuracy).ToString());
        if (property == "Agr")
        {
            Console.WriteLine("Agr = " + section.Agr.ToString());
            value.Should().BeApproximately(section.Agr / 1000, value * _accuracy);
        }
        if (property == "RMinor")
        {
            Console.WriteLine("RMinor = " + section.RMinor.ToString());
            value.Should().BeApproximately(section.RMinor, value * _accuracy);
        }
        if (property == "RMajor")
        {
            Console.WriteLine("RMajor = " + section.RMajor.ToString());
            value.Should().BeApproximately(section.RMajor, value * _accuracy);
        }
        if (property == "AMinor")
        {
            Console.WriteLine("AMinor = " + section.AMinor.ToString());
            value.Should().BeApproximately(section.AMinor, value * _accuracy);
        }
        if (property == "AMajor")
        {
            Console.WriteLine("AMajor = " + section.AMajor.ToString());
            value.Should().BeApproximately(section.AMajor, value * _accuracy);
        }
        if (property == "CeMinor")
        {
            Console.WriteLine("CeMinor = " + section.CeMinor.ToString());
            value.Should().BeApproximately(section.CeMinor, value * _accuracy);
        }
        if (property == "CeMajor")
        {
            Console.WriteLine("CeMajor = " + section.CeMajor.ToString());
            value.Should().BeApproximately(section.CeMajor, value * _accuracy);
        }
        if (property == "ZeMajor")
        {
            Console.WriteLine("ZeMajor = " + section.ZeMajor.ToString());
            value.Should().BeApproximately(section.ZeMajor / 1000, value * _accuracy);
        }
        if (property == "ZeMinor")
        {
            Console.WriteLine("ZeMinor = " + section.ZeMinor.ToString());
            value.Should().BeApproximately(section.ZeMinor / 1000, value * _accuracy);
        }
        if (property == "ZplMajor")
        {
            Console.WriteLine("ZplMajor = " + section.ZplMajor.ToString());
            value.Should().BeApproximately(section.ZplMajor / 1000, value * _accuracy);
        }
        if (property == "ZplMinor")
        {
            Console.WriteLine("ZplMinor = " + section.ZplMinor.ToString());
            value.Should().BeApproximately(section.ZplMinor / 1000, value * _accuracy);
        }
        if (property == "IMajor")
        {
            Console.WriteLine("IMajor = " + section.IMajor.ToString());
            value.Should().BeApproximately(section.IMajor / 1000000, value * _accuracy);
        }
        if (property == "IMinor")
        {
            Console.WriteLine("IMinor = " + section.IMinor.ToString());
            value.Should().BeApproximately(section.IMinor / 1000000, value * _accuracy);
        }
        if (property == "J")
        {
            Console.WriteLine("J = " + section.J.ToString());
            Console.WriteLine("Adjusted Accuracy = " + (value * _accuracy).ToString());
            if (section.SectionType == SectionType.CircularHollow || section.SectionType == SectionType.RectangularHollow)
                value.Should().BeApproximately(section.J / 1000000, value * _accuracy);
            else
                value.Should().BeApproximately(section.J / 1000, value * _accuracy);
        }
        if (property == "Cw")
        {
            Console.WriteLine("Cw = " + section.Cw.ToString());
            value.Should().BeApproximately(section.Cw / 1000000000, value * _accuracy);
        }
        if (property == "Cx")
        {
            Console.WriteLine("D = " + section.D.ToString());
            Console.WriteLine("CeMajor = " + section.CeMajor.ToString());
            value.Should().BeApproximately(section.D - section.CeMajor, value * _accuracy);
        }
    }
}
