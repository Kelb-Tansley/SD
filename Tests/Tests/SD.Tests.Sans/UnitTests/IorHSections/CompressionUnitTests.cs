using Microsoft.VisualStudio.TestTools.UnitTesting;
using SD.Core.Shared.Models.BeamModels.Sections;
using SD.Element.Design.Sans.Engine;

namespace SD.Tests.Sans.UnitTests.IorHSections;

[TestClass]
public class CompressionUnitTests : UnitTestsBase
{
    const double _accuracy = 0.02; //2%

    [TestMethod]
    public void Class4WebDoublySymmetricISectionCompressiveResistance()
    {
        //Taken from red book example. File Doubly symmetric I section - class 4 web.mcdx

        //Inputs
        var l = 3500;

        var section = new IorHSection(
            b1: 101.6D,
            b2: 101.6D,
            d: 305.1D,
            t1: 6.7D,
            t2: 6.7D,
            t3: 5.8D,
            GetMaterialProperties(),
            j: 4.80E+04D
        );
        var beam = GetBeam(l, section);

        var value = 162000D;
        var classification = ClassificationService.ClassifyAxialCompression(beam.Section);
        var cr = CompressionService.CompressiveResistance(beam, classification, value);

        //Assert input vs output
        Assert.AreEqual(cr.Cr, value, _accuracy * value);
    }

    [TestMethod]
    public void Class4FlangeDoublySymmetricHSectionCompressiveResistance()
    {
        //Taken from red book example. File Doubly symmetric H section - class 4 flange.mcdx

        //Inputs
        var l = 0.0001;

        var section = new IorHSection(
            b1: 152.4D,
            b2: 152.4D,
            d: 152.4D,
            t1: 6.8D,
            t2: 6.8D,
            t3: 6.1D,
            GetMaterialProperties(),
            j: 4.84E+04D
        );
        var beam = GetBeam(l, section);

        var value = 889555D;
        var classification = ClassificationService.ClassifyAxialCompression(beam.Section);
        var cr = CompressionService.CompressiveResistance(beam, classification, value);

        //Assert input vs output
        Assert.AreEqual(cr.Cr, value, _accuracy * value);
    }

    [TestMethod]
    public void Class4Cu0FlangeDoublySymmetricHSectionCompressiveResistance()
    {
        //Taken from red book example. File Doubly symmetric H section - class 4 flange.mcdx

        //Inputs
        var l = 0.0001;

        var section = new IorHSection(
            b1: 152.4D,
            b2: 152.4D,
            d: 152.4D,
            t1: 6.8D,
            t2: 6.8D,
            t3: 6.1D,
            GetMaterialProperties(),
            j: 4.84E+04D
        );
        var beam = GetBeam(l, section);

        var classification = ClassificationService.ClassifyAxialCompression(beam.Section);
        var cr = CompressionService.CompressiveResistance(beam, classification, 0.0001D);

        //Assert input vs output
        var value = 919586D;
        Assert.AreEqual(cr.Cr, value, _accuracy * value);
    }


    [TestMethod]
    public void NonClass4DoublySymmetricHSectionCompressiveResistance()
    {
        //Taken from red book example. File Doubly symmetric H section - non class 4.mcdx

        //Inputs
        var l = 8000;

        var section = new IorHSection(
            b1: 258.3D,
            b2: 258.3D,
            d: 266.7D,
            t1: 20.5D,
            t2: 20.5D,
            t3: 13.0D,
            GetMaterialProperties(20.5, 20.5),
            j: 1.75E+06D
        );
        var beam = GetBeam(l, section);

        var value = 1370000D;
        var classification = ClassificationService.ClassifyAxialCompression(beam.Section);
        var cr = CompressionService.CompressiveResistance(beam, classification, value);

        //Assert input vs output
        Assert.AreEqual(cr.Cr, value, _accuracy * value);
    }
}