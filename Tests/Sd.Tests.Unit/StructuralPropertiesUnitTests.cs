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
        Assert.AreEqual(section.ZplMajor / 1000, 1601.94D, _accuracy);
        Assert.AreEqual(section.ZeMajor / 1000, 1322.64D, _accuracy);
    }

    [TestMethod]
    public void CalculatePlasticSectionModulusFailsBelowWeb()
    {
        //Run test function
        Assert.ThrowsException<NotImplementedException>(() => new IorHSection(
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
        Assert.ThrowsException<NotImplementedException>(() => new IorHSection(
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
        Assert.AreEqual(section.Agr, 8200D, _accuracy);
        Assert.AreEqual(section.IMajor / 1000000, 137.929D, _accuracy);
        Assert.AreEqual(section.IMinor / 1000, 14193.33D, _accuracy);
        Assert.AreEqual(section.ZeMajor / 1000, 592.465D, _accuracy);
        Assert.AreEqual(section.ZeMinor / 1000, 141.933D, _accuracy);
        Assert.AreEqual(section.ZplMajor / 1000, 876D, _accuracy);
        Assert.AreEqual(section.ZplMinor / 1000, 233D, _accuracy);
        Assert.AreEqual(section.RMajor, 129.694D, _accuracy);
        Assert.AreEqual(section.RMinor, 41.604D, _accuracy);
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
        Assert.AreEqual(section.Agr, 5703D, _accuracy);
        Assert.AreEqual(section.IMajor / 1000000, 80.65759725D, _accuracy);
        Assert.AreEqual(section.IMinor / 1000, 5644.86D, _accuracy);
        Assert.AreEqual(section.ZeMajor / 1000, 537.717D, _accuracy);
        Assert.AreEqual(section.ZeMinor / 1000, 81.6064D, _accuracy);
        Assert.AreEqual(section.ZplMajor / 1000, 628.175D, _accuracy);
        Assert.AreEqual(section.ZplMinor / 1000, 148.091D, _accuracy);
        Assert.AreEqual(section.RMajor, 118.924D, _accuracy);
        Assert.AreEqual(section.RMinor, 31.4612D, _accuracy);
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
        Assert.AreEqual(section.Agr, 5043D, _accuracy);
        Assert.AreEqual(section.IMajor / 1000000, 67.3812D, _accuracy);
        Assert.AreEqual(section.IMinor / 1000, 3009.57D, _accuracy);
        Assert.AreEqual(section.ZeMajor / 1000, 449.208D, _accuracy);
        Assert.AreEqual(section.ZeMinor / 1000, 52.8776D, _accuracy);
        Assert.AreEqual(section.ZplMajor / 1000, 534.620D, _accuracy);
        Assert.AreEqual(section.ZplMinor / 1000, 95.2203D, _accuracy);
        Assert.AreEqual(section.RMajor, 115.591D, _accuracy);
        Assert.AreEqual(section.RMinor, 24.4291D, _accuracy);
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
        Assert.AreEqual(section.Agr, 1700D, _accuracy);
        Assert.AreEqual(section.Ixx / 1000000, 1.29181D, _accuracy);
        Assert.AreEqual(section.Iyy / 1000000, 1.29181D, _accuracy);
        Assert.AreEqual(section.IMajor / 1000000, 2.05417D, _accuracy);
        Assert.AreEqual(section.IMinor / 1000, 529.461D, _accuracy);
        Assert.AreEqual(section.Zxx, 20240.4D, _accuracy);
        Assert.AreEqual(section.Zyy, 20240.4D, _accuracy);
        Assert.AreEqual(section.ZeMajor, 32278.11D, _accuracy);
        Assert.AreEqual(section.ZeMinor, 14302.36D, _accuracy);
        Assert.AreEqual(section.RMajor, 34.7611D, _accuracy);
        Assert.AreEqual(section.RMinor, 17.6479D, _accuracy);
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
        Assert.AreEqual(section.Agr, 2150D, _accuracy);
        Assert.AreEqual(section.IMajor / 1000000, 5.35650D, _accuracy);
        Assert.AreEqual(section.IMinor / 1000, 562.581D, _accuracy);
        Assert.AreEqual(section.Zxx / 1000, 52.4112D, _accuracy);
        Assert.AreEqual(section.Zyy / 1000, 14.9852D, _accuracy);
        Assert.AreEqual(section.ZeMajor / 1000, 55.1466D, _accuracy);
        Assert.AreEqual(section.ZeMinor / 1000, 12.3836D, _accuracy);
        Assert.AreEqual(section.RMajor, 49.9139D, _accuracy);
        Assert.AreEqual(section.RMinor, 16.1761D, _accuracy);
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
        Assert.AreEqual(section.Agr, 1256D, section.Agr * accuracy);
        Assert.AreEqual(section.CeMajor, 33.2994D, section.CeMajor * accuracy);
        Assert.AreEqual(section.CeMinor, 15.7994D, section.CeMinor * accuracy);
        Assert.AreEqual(section.Ixx / 1000000, 1.28368D, (section.Ixx / 1000000) * accuracy);
        Assert.AreEqual(section.Iyy / 1000, 434.512D, (section.Iyy / 1000) * accuracy);
        Assert.AreEqual(section.IMajor / 1000000, 1.46640D, (section.IMajor / 1000000) * accuracy);
        Assert.AreEqual(section.IMinor / 1000, 251.795D, (section.IMinor / 1000) * accuracy);
        Assert.AreEqual(section.Zxx / 1000, 19.2454D, (section.Zxx / 1000) * accuracy);
        Assert.AreEqual(section.Zyy / 1000, 8.83143D, (section.Zyy / 1000) * accuracy);
        Assert.AreEqual(section.RMajor, 34.1689D, section.RMajor * accuracy);
        Assert.AreEqual(section.RMinor, 14.1589D, section.RMinor * accuracy);
        Assert.AreEqual(section.Alpha, 22.8213D, section.Alpha * accuracy);

        accuracy = 0.025;
        Assert.AreEqual(section.ZeMajor / 1000, 21.690D, (section.ZeMajor / 1000) * accuracy);
        Assert.AreEqual(section.ZeMinor / 1000, 7.08556D, (section.ZeMinor / 1000) * accuracy);
        Assert.AreEqual(section.V1, 26.9D, section.V1 * accuracy);
        Assert.AreEqual(section.V2, 34.7D, section.V2 * accuracy);
        Assert.AreEqual(section.U1, 68.1D, section.U1 * accuracy);
        Assert.AreEqual(section.U2, 49.2D, section.U2 * accuracy);
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
        Assert.AreEqual(section.Agr, 8364.54D, section.Agr * accuracy);
        Assert.AreEqual(section.CeMajor, 130.5D, section.CeMajor * accuracy);
        Assert.AreEqual(section.CeMinor, 111.0003D, section.CeMinor * accuracy);
        Assert.AreEqual(section.IMajor / 1000, 37521.0D, (section.IMajor / 1000) * accuracy);
        Assert.AreEqual(section.IMinor / 1000, 8861.88D, (section.IMinor / 1000) * accuracy);
        Assert.AreEqual(section.RMajor, 66.9755D, section.RMajor * accuracy);
        Assert.AreEqual(section.RMinor, 32.5493D, section.RMinor * accuracy);

        //accuracy = 0.025;
        Assert.AreEqual(section.ZeMajor / 1000, 287.517D, (section.ZeMajor / 1000) * accuracy);
        Assert.AreEqual(section.ZeMinor / 1000, 79.8365D, (section.ZeMinor / 1000) * accuracy);
        Assert.AreEqual(section.ZplMajor / 1000, 437.734D, (section.ZplMajor / 1000) * accuracy);
        Assert.AreEqual(section.ZplMinor / 1000, 160.496D, (section.ZplMinor / 1000) * accuracy);
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
        Assert.AreEqual(section.Agr, 1535.36D, section.Agr * accuracy);
        Assert.AreEqual(section.CeMajor, 107.0884D, section.CeMajor * accuracy);
        Assert.AreEqual(section.CeMinor, 50.8D, section.CeMinor * accuracy);
        Assert.AreEqual(section.IMajor / 1000, 3700.97D, (section.IMajor / 1000) * accuracy);
        Assert.AreEqual(section.IMinor / 1000, 596.672D, (section.IMinor / 1000) * accuracy);
        Assert.AreEqual(section.RMajor, 49.0967D, section.RMajor * accuracy);
        Assert.AreEqual(section.RMinor, 19.7134D, section.RMinor * accuracy);

        //accuracy = 0.025;
        Assert.AreEqual(section.ZeMajor / 1000, 34.5599D, (section.ZeMajor / 1000) * accuracy);
        Assert.AreEqual(section.ZeMinor / 1000, 11.7455D, (section.ZeMinor / 1000) * accuracy);
        Assert.AreEqual(section.ZplMajor / 1000, 62.8102D, (section.ZplMajor / 1000) * accuracy);
        Assert.AreEqual(section.ZplMinor / 1000, 18.7728D, (section.ZplMinor / 1000) * accuracy);
    }

    private static Material GetMaterial(double t1, double t2, int t3)
     => new(fyElement1: t1 > 16 ? 345D : 350D, fyElement2: t2 > 16 ? 345D : 350D, fyElement3: t3 > 16 ? 345D : 350D)
     {
         Es = 200000,
         Gs = 77000
     };
}