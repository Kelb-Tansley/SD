using Microsoft.VisualStudio.TestTools.UnitTesting;
using SD.Core.Shared.Models.BeamModels.Sections;
using SD.Element.Design.Sans.Engine;
using SD.Element.Design.Sans.Models;

namespace SD.Tests.Sans.UnitTests.RectangularSections;

[TestClass]
public class CompressionUnitTests : UnitTestsBase
{
    const double _accuracy = 0.03; //4%
    const double _class4accuracy = 0.15; //15%

    [TestMethod]
    public void NonClass4SquareSectionCompressiveResistance()
    {
        //Taken from red book example. File Doubly symmetric H section - non class 4.mcdx

        //Inputs
        var l = 5000;

        var section = new RectangularSection(
            b: 200,
            d: 200,
            t1: 6,
            t2: 6,
            GetMaterialProperties(6, 6, 0)
        );
        var beam = GetBeam(l, section);

        var value = 994000D;
        var cr = CompressionService.CompressiveResistance(beam, (beam.Resistance as SansBeamResistance).CompressionClass, value);

        //Assert input vs output
        Assert.AreEqual(cr.Cr, value, _accuracy * cr.Cr);
    }

    [TestMethod]
    public void NonClass4RectangularSectionCompressiveResistance()
    {
        //Taken from red book example. File Doubly symmetric H section - non class 4.mcdx

        //Inputs
        var l = 3000;

        var section = new RectangularSection(
            b: 40,
            d: 60,
            t1: 2.5,
            t2: 2.5,
            GetMaterialProperties(2.5, 2.5, 0)
        );
        var beam = GetBeam(l, section);

        var value = 22400D;
        var cr = CompressionService.CompressiveResistance(beam, (beam.Resistance as SansBeamResistance).CompressionClass, value);

        //Assert input vs output
        Assert.AreEqual(cr.Cr, value, _accuracy * cr.Cr);
    }

    [TestMethod]
    public void Class4WebRectangularSectionCompressiveResistance()
    {
        //Taken from red book example. File Doubly symmetric I section - class 4 web.mcdx

        //Inputs
        var l = 3500;

        var section = new RectangularSection(
            b: 180,
            d: 340,
            t1: 6,
            t2: 6,
            GetMaterialProperties(6, 6, 0)
        );
        var beam = GetBeam(l, section);

        var value = 1372000D;
        var cr = CompressionService.CompressiveResistance(beam, (beam.Resistance as SansBeamResistance).CompressionClass, 0.0001D);

        //Assert input vs output
        Assert.AreEqual(cr.Cr, value, _class4accuracy * cr.Cr);
    }

    [TestMethod]
    public void Class4WebAndFlangeSquareSectionCompressiveResistance()
    {
        //Taken from red book example. File Doubly symmetric I section - class 4 web.mcdx

        //Inputs
        var l = 3500;

        var section = new RectangularSection(
            b: 175,
            d: 175,
            t1: 4,
            t2: 4,
            GetMaterialProperties(4, 4, 0)
        );
        var beam = GetBeam(l, section);

        var value = 687000D;
        var cr = CompressionService.CompressiveResistance(beam, (beam.Resistance as SansBeamResistance).CompressionClass, value);

        //Assert input vs output
        Assert.AreEqual(cr.Cr, value, _class4accuracy * cr.Cr);
    }

}