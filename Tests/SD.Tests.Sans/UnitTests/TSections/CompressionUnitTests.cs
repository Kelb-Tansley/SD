using Microsoft.VisualStudio.TestTools.UnitTesting;
using SD.Core.Shared.Models.BeamModels.Sections;
using SD.Element.Design.Sans.Engine;

namespace SD.Tests.Sans.UnitTests.TSections;

[TestClass]
public class CompressionUnitTests : UnitTestsBase
{
    const double _accuracy = 0.01; //1%
    const double _class4accuracy = 0.06; //6%

    [TestMethod]
    public void Class4StemMonoSymmetricTSectionCompressiveResistance()
    {
        //Taken from red book example. File Doubly symmetric I section - class 4 web.mcdx

        //Inputs
        var l = 5000;

        var section = new TSection(
            b: 177.6,
            d: 201.3,
            t1: 10.9,
            t2: 7.6,
            GetMaterialProperties(10.9, 7.6, 0)
        );
        var beam = GetBeam(l, section);

        var value = 265000D;
        var classification = ClassificationService.ClassifyAxialCompression(beam.Section);
        var cr = CompressionService.CompressiveResistance(beam, classification, value);

        //Assert input vs output
        Assert.AreEqual(cr.Cr, value, _class4accuracy * cr.Cr);
    }

    [TestMethod]
    public void Class4FlangeMonoSymmetricTSectionCompressiveResistance()
    {
        //Taken from red book example. File Doubly symmetric H section - class 4 flange.mcdx

        //Inputs
        var l = 2000;

        var section = new TSection(
            b: 152.4,
            d: 76.2,
            t1: 6.8,
            t2: 6.1,
            GetMaterialProperties(6.8, 6.1, 0)
        );
        var beam = GetBeam(l, section);

        var value = 202000D;
        var classification = ClassificationService.ClassifyAxialCompression(beam.Section);
        var cr = CompressionService.CompressiveResistance(beam, classification, value);

        //Assert input vs output
        Assert.AreEqual(cr.Cr, value, _accuracy * cr.Cr);
    }

    [TestMethod]
    public void NonClass4MonoSymmetricTSectionCompressiveResistance()
    {
        //Taken from red book example. File Doubly symmetric H section - class 4 flange.mcdx

        //Inputs
        var l = 7000;

        var section = new TSection(
            b: 304.8,
            d: 153.9,
            t1: 15.4,
            t2: 9.9,
            GetMaterialProperties(15.4, 9.9, 0)
        );
        var beam = GetBeam(l, section);

        var classification = ClassificationService.ClassifyAxialCompression(beam.Section);
        var cr = CompressionService.CompressiveResistance(beam, classification, 0.0001D);

        //Assert input vs output
        var value = 292000D;
        Assert.AreEqual(cr.Cr, value, _accuracy * cr.Cr);
    }

}