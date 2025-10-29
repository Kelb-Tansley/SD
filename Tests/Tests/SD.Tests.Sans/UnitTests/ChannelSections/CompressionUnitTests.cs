using Microsoft.VisualStudio.TestTools.UnitTesting;
using SD.Core.Shared.Models.BeamModels.Sections;
using SD.Element.Design.Sans.Engine;

namespace SD.Tests.Sans.UnitTests.ChannelSections;

[TestClass]
public class CompressionUnitTests : UnitTestsBase
{
    const double _accuracy = 0.02; //2%

    [TestMethod]
    public void Class4WebMonosymmetricChannelCompressiveResistance()
    {
        //Taken from red book example. File Doubly symmetric I section - class 4 web.mcdx

        //Inputs
        var l = 2000D;

        var section = new ChannelSection(
            b: 90,
            d: 230,
            t1: 14,
            t2: 7.5,
            GetMaterialProperties()
        );
        var beam = GetBeam(l, section);

        var value = 824000D;
        var classification = ClassificationService.ClassifyAxialCompression(beam.Section);
        var cr = CompressionService.CompressiveResistance(beam, classification, value);

        //Assert input vs output
        Assert.AreEqual(cr.Cr, value, _accuracy * value);
    }
}