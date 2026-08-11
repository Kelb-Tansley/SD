using SD.Core.Shared.Models.BeamModels;
using SD.Core.Shared.Models.BeamModels.Sections;

namespace SD.Tests.Unit;

[TestClass]
public class StructuralPropertiesUnitTests
{
    const double _accuracy = 0.01;

    [TestMethod]
    public void CalculatePlasticSectionModulus()
    {
        //Taken from text book example. File Mono-symmetric I beam.jpg
        //Inputs
        var section = new IorHSection(
            b1: 180D,
            b2: 240D,
            d: 430D,
            t1: 15D,
            t2: 15D,
            t3: 8D,
            GetMaterial(0, 0, 0)
        );

        //Assert input vs output
        Assert.AreEqual(1601.94D, section.ZplMajor / 1000, _accuracy);
        Assert.AreEqual(1322.64D, section.ZeMajor / 1000, _accuracy);
    }

    [TestMethod]
    public void CalculatePlasticSectionModulusFailsBelowWeb()
    {
        //Run test function
        Assert.ThrowsExactly<NotImplementedException>(() => new IorHSection(
            b1: 180D,
            b2: 240D,
            d: 450D,
            t1: 40D,
            t2: 15D,
            t3: 8D,
            GetMaterial(0, 0, 0)
        ));
    }

    [TestMethod]
    public void CalculatePlasticSectionModulusFailAboveWeb()
    {
        //Run test function
        Assert.ThrowsExactly<NotImplementedException>(() => new IorHSection(
            b1: 180D,
            b2: 240D,
            d: 450D,
            t1: 15D,
            t2: 85D,
            t3: 8D,
            GetMaterial(0, 0, 0)
        ));
    }

    [TestMethod]
    public void CalculateMonoSymmetricISectionProperties()
    {
        //Taken from web example. https://calcresource.com/cross-section-doubletee-unsym.html
        //Inputs
        var section = new IorHSection(
            b1: 200D,
            b2: 100D,
            d: 350D,
            t1: 20D,
            t2: 10D,
            t3: 10D,
            GetMaterial(0, 0, 0)
        );

        //Assert input vs output
        Assert.AreEqual(8200D, section.Agr, _accuracy);
        Assert.AreEqual(137.929D, section.IMajor / 1000000, _accuracy);
        Assert.AreEqual(14193.33D, section.IMinor / 1000, _accuracy);
        Assert.AreEqual(592.465D, section.ZeMajor / 1000, _accuracy);
        Assert.AreEqual(141.933D, section.ZeMinor / 1000, _accuracy);
        Assert.AreEqual(876D, section.ZplMajor / 1000, _accuracy);
        Assert.AreEqual(233D, section.ZplMinor / 1000, _accuracy);
        Assert.AreEqual(129.694D, section.RMajor, _accuracy);
        Assert.AreEqual(41.604D, section.RMinor, _accuracy);
    }

    [TestMethod]
    public void CalculateChannelCentroidOutsideWebSectionProperties()
    {
        //Taken from web example and red book standard section. https://calcresource.com/cross-section-channel.html
        //Inputs
        var section = new ChannelSection(
            b: 100D,
            d: 300D,
            t1: 16.5D,
            t2: 9D,
            GetMaterial(0, 0, 0)
        );

        //Assert input vs output
        Assert.AreEqual(5703D, section.Agr, _accuracy);
        Assert.AreEqual(80.65759725D, section.IMajor / 1000000, _accuracy);
        Assert.AreEqual(5644.86D, section.IMinor / 1000, _accuracy);
        Assert.AreEqual(537.717D, section.ZeMajor / 1000, _accuracy);
        Assert.AreEqual(81.6064D, section.ZeMinor / 1000, _accuracy);
        Assert.AreEqual(628.175D, section.ZplMajor / 1000, _accuracy);
        Assert.AreEqual(148.091D, section.ZplMinor / 1000, _accuracy);
        Assert.AreEqual(118.924D, section.RMajor, _accuracy);
        Assert.AreEqual(31.4612D, section.RMinor, _accuracy);
    }

    [TestMethod]
    public void CalculateChannelCentroidInsideWebSectionProperties()
    {
        //Taken from web example. https://calcresource.com/cross-section-channel.html
        //Inputs
        var section = new ChannelSection(
            b: 80D,
            d: 300D,
            t1: 16.5D,
            t2: 9D,
            GetMaterial(0, 0, 0)
        );

        //Assert input vs output
        Assert.AreEqual(5043D, section.Agr, _accuracy);
        Assert.AreEqual(67.3812D, section.IMajor / 1000000, _accuracy);
        Assert.AreEqual(3009.57D, section.IMinor / 1000, _accuracy);
        Assert.AreEqual(449.208D, section.ZeMajor / 1000, _accuracy);
        Assert.AreEqual(52.8776D, section.ZeMinor / 1000, _accuracy);
        Assert.AreEqual(534.620D, section.ZplMajor / 1000, _accuracy);
        Assert.AreEqual(95.2203D, section.ZplMinor / 1000, _accuracy);
        Assert.AreEqual(115.591D, section.RMajor, _accuracy);
        Assert.AreEqual(24.4291D, section.RMinor, _accuracy);
    }

    [TestMethod]
    public void CalculateEqualAngleSectionProperties()
    {
        //Taken from web example and red book standard section. https://calcresource.com/cross-section-angle.html
        //Inputs
        var section = new AngleSection(
            b: 90D,
            d: 90D,
            t: 10D,
            GetMaterial(0, 0, 0)
        );

        //Assert input vs output
        Assert.AreEqual(1700D, section.Agr, _accuracy);
        Assert.AreEqual(1.29181D, section.Ixx / 1000000, _accuracy);
        Assert.AreEqual(1.29181D, section.Iyy / 1000000, _accuracy);
        Assert.AreEqual(2.05417D, section.IMajor / 1000000, _accuracy);
        Assert.AreEqual(529.461D, section.IMinor / 1000, _accuracy);
        Assert.AreEqual(20240.4D, section.Zxx, _accuracy);
        Assert.AreEqual(20240.4D, section.Zyy, _accuracy);
        Assert.AreEqual(32278.11D, section.ZeMajor, _accuracy);
        Assert.AreEqual(14302.36D, section.ZeMinor, _accuracy);
        Assert.AreEqual(34.7611D, section.RMajor, _accuracy);
        Assert.AreEqual(17.6479D, section.RMinor, _accuracy);
    }

    [TestMethod]
    public void CalculateUnEqualAngleSectionProperties()
    {
        //Taken from web example and red book standard section. https://calcresource.com/cross-section-angle.html
        //Inputs
        var section = new AngleSection(
            b: 75D,
            d: 150D,
            t: 10D,
            GetMaterial(0, 0, 0)
        );

        //Assert input vs output
        Assert.AreEqual(2150D, section.Agr, _accuracy);
        Assert.AreEqual(5.35650D, section.IMajor / 1000000, _accuracy);
        Assert.AreEqual(562.581D, section.IMinor / 1000, _accuracy);
        Assert.AreEqual(52.4112D, section.Zxx / 1000, _accuracy);
        Assert.AreEqual(14.9852D, section.Zyy / 1000, _accuracy);
        Assert.AreEqual(55.1466D, section.ZeMajor / 1000, _accuracy);
        Assert.AreEqual(12.3836D, section.ZeMinor / 1000, _accuracy);
        Assert.AreEqual(49.9139D, section.RMajor, _accuracy);
        Assert.AreEqual(16.1761D, section.RMinor, _accuracy);
    }

    [TestMethod]
    public void CalculateUnEqualAngle2SectionProperties()
    {
        //Taken from web example and red book standard section. https://calcresource.com/cross-section-angle.html
        //Inputs
        var section = new AngleSection(
            b: 65D,
            d: 100D,
            t: 8D,
            GetMaterial(0, 0, 0)
        );

        //Assert input vs output
        var accuracy = 0.00001; // 0.001%
        Assert.AreEqual(1256D, section.Agr, section.Agr * accuracy);
        Assert.AreEqual(33.2994D, section.CeMajor, section.CeMajor * accuracy);
        Assert.AreEqual(15.7994D, section.CeMinor, section.CeMinor * accuracy);
        Assert.AreEqual(1.28368D, section.Ixx / 1000000, (section.Ixx / 1000000) * accuracy);
        Assert.AreEqual(434.512D, section.Iyy / 1000, (section.Iyy / 1000) * accuracy);
        Assert.AreEqual(1.46640D, section.IMajor / 1000000, (section.IMajor / 1000000) * accuracy);
        Assert.AreEqual(251.795D, section.IMinor / 1000, (section.IMinor / 1000) * accuracy);
        Assert.AreEqual(19.2454D, section.Zxx / 1000, (section.Zxx / 1000) * accuracy);
        Assert.AreEqual(8.83143D, section.Zyy / 1000, (section.Zyy / 1000) * accuracy);
        Assert.AreEqual(34.1689D, section.RMajor, section.RMajor * accuracy);
        Assert.AreEqual(14.1589D, section.RMinor, section.RMinor * accuracy);
        Assert.AreEqual(22.8213D, section.Alpha, section.Alpha * accuracy);

        accuracy = 0.025;
        Assert.AreEqual(21.690D, section.ZeMajor / 1000, (section.ZeMajor / 1000) * accuracy);
        Assert.AreEqual(7.08556D, section.ZeMinor / 1000, (section.ZeMinor / 1000) * accuracy);
        Assert.AreEqual(26.9D, section.V1, section.V1 * accuracy);
        Assert.AreEqual(34.7D, section.V2, section.V2 * accuracy);
        Assert.AreEqual(68.1D, section.U1, section.U1 * accuracy);
        Assert.AreEqual(49.2D, section.U2, section.U2 * accuracy);
    }

    [TestMethod]
    public void CalculateTSectionPNAInsideFlangeYYMajorProperties()
    {
        //Taken from web example and red book standard section. https://calcresource.com/cross-section-tee.html
        //Inputs
        var section = new TSection(
            b: 261D,
            d: 138.2D,
            t1: 25.3D,
            t2: 15.6D,
            GetMaterial(0, 0, 0)
        );

        //Assert input vs output
        var accuracy = 0.0001; // 0.001%
        Assert.AreEqual(8364.54D, section.Agr, section.Agr * accuracy);
        Assert.AreEqual(130.5D, section.CeMajor, section.CeMajor * accuracy);
        Assert.AreEqual(111.0003D, section.CeMinor, section.CeMinor * accuracy);
        Assert.AreEqual(37521.0D, section.IMajor / 1000, (section.IMajor / 1000) * accuracy);
        Assert.AreEqual(8861.88D, section.IMinor / 1000, (section.IMinor / 1000) * accuracy);
        Assert.AreEqual(66.9755D, section.RMajor, section.RMajor * accuracy);
        Assert.AreEqual(32.5493D, section.RMinor, section.RMinor * accuracy);

        //accuracy = 0.025;
        Assert.AreEqual(287.517D, section.ZeMajor / 1000, (section.ZeMajor / 1000) * accuracy);
        Assert.AreEqual(79.8365D, section.ZeMinor / 1000, (section.ZeMinor / 1000) * accuracy);
        Assert.AreEqual(437.734D, section.ZplMajor / 1000, (section.ZplMajor / 1000) * accuracy);
        Assert.AreEqual(160.496D, section.ZplMinor / 1000, (section.ZplMinor / 1000) * accuracy);
    }

    [TestMethod]
    public void CalculateTSectionPNAOutsideFlangeXXMajorProperties()
    {
        //Taken from web example and red book standard section. https://calcresource.com/cross-section-tee.html
        //Inputs
        var section = new TSection(
            b: 101.6D,
            d: 152.4D,
            t1: 6.8D,
            t2: 5.8D,
            GetMaterial(0, 0, 0)
        );

        //Assert input vs output
        var accuracy = 0.0001; // 0.001%
        Assert.AreEqual(1535.36D, section.Agr, section.Agr * accuracy);
        Assert.AreEqual(107.0884D, section.CeMajor, section.CeMajor * accuracy);
        Assert.AreEqual(50.8D, section.CeMinor, section.CeMinor * accuracy);
        Assert.AreEqual(3700.97D, section.IMajor / 1000, (section.IMajor / 1000) * accuracy);
        Assert.AreEqual(596.672D, section.IMinor / 1000, (section.IMinor / 1000) * accuracy);
        Assert.AreEqual(49.0967D, section.RMajor, section.RMajor * accuracy);
        Assert.AreEqual(19.7134D, section.RMinor, section.RMinor * accuracy);

        //accuracy = 0.025;
        Assert.AreEqual(34.5599D, section.ZeMajor / 1000, (section.ZeMajor / 1000) * accuracy);
        Assert.AreEqual(11.7455D, section.ZeMinor / 1000, (section.ZeMinor / 1000) * accuracy);
        Assert.AreEqual(62.8102D, section.ZplMajor / 1000, (section.ZplMajor / 1000) * accuracy);
        Assert.AreEqual(18.7728D, section.ZplMinor / 1000, (section.ZplMinor / 1000) * accuracy);
    }

    private static Material GetMaterial(double t1, double t2, int t3)
     => new(fyElement1: t1 > 16 ? 345D : 350D, fyElement2: t2 > 16 ? 345D : 350D, fyElement3: t3 > 16 ? 345D : 350D)
     {
         Es = 200000,
         Gs = 77000
     };
}