using Microsoft.VisualStudio.TestTools.UnitTesting;
using SD.Core.Shared.Models.BeamModels.Sections;
using SD.Element.Design.Sans.Engine;
using SD.Element.Design.Sans.Models;

namespace SD.Tests.Sans.UnitTests.CircularSections;

[TestClass]
public class CompressionUnitTests : UnitTestsBase
{
    const double _accuracy = 0.01; //1%

    [TestMethod]
    public void NonClass4CircularSectionCompressiveResistance()
    {
        //Taken from red book example. File Doubly symmetric H section - non class 4.mcdx

        //Inputs
        var l = 5000;

        var section = new CircularSection(
            d: 219.1D,
            t: 3.5D,
            GetMaterialProperties()
        );
        var beam = GetBeam(l, section);

        var value = 508000D;
        var classification = ClassificationService.ClassifyAxialCompression(beam.Section);
        var cr = CompressionService.CompressiveResistance(beam, classification, value);

        //Assert input vs output
        Assert.AreEqual(cr.Cr, value, _accuracy * cr.Cr);
    }
    [TestMethod]
    public void Class4CircularSectionCompressiveResistance()
    {
        //Taken from red book example. File Doubly symmetric I section - class 4 web.mcdx

        //Inputs
        var l = 3500;

        var section = new CircularSection(
            d: 457D,
            t: 6D,
            GetMaterialProperties()
        );
        var beam = GetBeam(l, section);

        var value = 2259000D;
        var cr = CompressionService.CompressiveResistance(beam, (beam.Resistance as SansBeamResistance).CompressionClass, value);

        //Assert input vs output
        Assert.AreEqual(cr.Cr, value, _accuracy * value);
    }
}