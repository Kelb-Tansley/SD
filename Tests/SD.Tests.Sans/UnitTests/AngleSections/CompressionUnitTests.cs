using Microsoft.VisualStudio.TestTools.UnitTesting;
using SD.Core.Shared.Models.BeamModels.Sections;
using SD.Element.Design.Sans.Engine;

namespace SD.Tests.Sans.UnitTests.AngleSections;

[TestClass]
public class CompressionUnitTests : UnitTestsBase
{
    const double _accuracy = 0.055; //5.5%

    [TestMethod]
    public void UnequalAngleCompressiveResistance()
    {
        //Taken from red book example. File Unequal leg angle - non class 4.mcdx

        //Inputs
        var l = 2000;

        var section = new AngleSection(
            b: 65,
            d: 100,
            t: 10,
            GetMaterialProperties()
        );
        var beam = GetBeam(l, section);

        var value = 115000D;
        var classification = ClassificationService.ClassifyAxialCompression(beam.Section);
        var cr = CompressionService.CompressiveResistance(beam, classification, value);

        //Assert input vs output
        Assert.AreEqual(cr.Cr, value, _accuracy * value);
    }

    [TestMethod]
    public void UnequalClass4AngleCompressiveResistance()
    {
        //Taken from red book example. File Unequal leg angle - non class 4.mcdx

        //Inputs
        var l = 2000;

        var section = new AngleSection(
            b: 75,
            d: 125,
            t: 8,
            GetMaterialProperties()
        );
        var beam = GetBeam(l, section);

        var value = 144000D;
        var classification = ClassificationService.ClassifyAxialCompression(beam.Section);
        var cr = CompressionService.CompressiveResistance(beam, classification, value);

        //Assert input vs output
        Assert.AreEqual(cr.Cr, value, _accuracy * value);
    }

    [TestMethod]
    public void EqualAngleCompressiveResistance()
    {
        //Taken from red book example. File Equal leg angle - non class 4.mcdx

        //Inputs
        var l = 2000;

        var section = new AngleSection(
            b: 150,
            d: 150,
            t: 15,
            GetMaterialProperties()
        );
        var beam = GetBeam(l, section);

        var value = 883000D;
        var classification = ClassificationService.ClassifyAxialCompression(beam.Section);
        var cr = CompressionService.CompressiveResistance(beam, classification, value);

        //Assert input vs output
        Assert.AreEqual(cr.Cr, value, _accuracy * value);
    }

    [TestMethod]
    public void EqualClass4AngleCompressiveResistance()
    {
        //Taken from red book example. File Equal leg angle - non class 4.mcdx

        //Inputs
        var l = 2000;

        var section = new AngleSection(
            b: 100,
            d: 100,
            t: 8,
            GetMaterialProperties()
        );
        var beam = GetBeam(l, section);

        var value = 201000D;
        var classification = ClassificationService.ClassifyAxialCompression(beam.Section);
        var cr = CompressionService.CompressiveResistance(beam, classification, value);

        //Assert input vs output
        Assert.AreEqual(cr.Cr, value, _accuracy * value);
    }
}