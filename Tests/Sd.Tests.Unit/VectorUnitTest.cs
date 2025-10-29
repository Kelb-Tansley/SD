using SD.Element.Design.Services;

namespace SD.Tests.Unit;

[TestClass]
public class VectorUnitTest
{
    const double _doubleAccuracy = 0.0000000000001;
    const double _wolframVectorAccuracy = 0.00001;

    [TestMethod]
    public void Project1VectorOntoAPlaneTest()
    {
        //Input constants
        var vector = new double[3] { 0.401, 0.457, 0.794 };
        var planeNormal = new double[3] { -0.504, 0.691, 0.519 };

        //Run test function
        var projectedVector = VectorService.Normalize(VectorService.ProjectVectorOntoPlane(vector, planeNormal));

        //Expected outputs
        var expectedProjectedVector = new double[3] { 0.782495, 0.110488, 0.612776 };

        //Assert input vs output
        Assert.AreEqual(projectedVector[0], expectedProjectedVector[0], _wolframVectorAccuracy);
        Assert.AreEqual(projectedVector[1], expectedProjectedVector[1], _wolframVectorAccuracy);
        Assert.AreEqual(projectedVector[2], expectedProjectedVector[2], _wolframVectorAccuracy);
    }
    [TestMethod]
    public void Project2VectorOntoAPlaneTest()
    {
        //Input constants
        var vector = new double[3] { 0.254, -0.384, 0.888 };
        var planeNormal = new double[3] { -0.494, -0.272, 0.826 };

        //Run test function
        var projectedVector = VectorService.Normalize(VectorService.ProjectVectorOntoPlane(vector, planeNormal));

        //Expected outputs
        var expectedProjectedVector = new double[3] { 0.862806, -0.270965, 0.426784 };

        //Assert input vs output
        Assert.AreEqual(projectedVector[0], expectedProjectedVector[0], _wolframVectorAccuracy);
        Assert.AreEqual(projectedVector[1], expectedProjectedVector[1], _wolframVectorAccuracy);
        Assert.AreEqual(projectedVector[2], expectedProjectedVector[2], _wolframVectorAccuracy);
    }
    [TestMethod]
    public void Project3VectorOntoAPlaneTest()
    {
        //Input constants
        var vector = new double[3] { 0.047, 0.66, 0.75 };
        var planeNormal = new double[3] { -0.212, 0.977, -0.006 };

        //Run test function
        var projectedVector = VectorService.Normalize(VectorService.ProjectVectorOntoPlane(vector, planeNormal));

        //Expected outputs
        var expectedProjectedVector = new double[3] { 0.232748, 0.0564672, 0.970896 };

        //Assert input vs output
        Assert.AreEqual(projectedVector[0], expectedProjectedVector[0], _wolframVectorAccuracy);
        Assert.AreEqual(projectedVector[1], expectedProjectedVector[1], _wolframVectorAccuracy);
        Assert.AreEqual(projectedVector[2], expectedProjectedVector[2], _wolframVectorAccuracy);
    }
    /// <summary>
    /// Additional test found on YouTube
    /// https://www.google.com/search?q=how+to+project+a+vector+onto+a+plane&oq=how+to+project+a+vector+onto+a+plane&gs_lcrp=EgZjaHJvbWUqBwgAEAAYgAQyBwgAEAAYgAQyBwgBEAAYgAQyBwgCEAAYgAQyCAgDEAAYFhgeMggIBBAAGBYYHjIICAUQABgWGB4yCAgGEAAYFhgeMggIBxAAGBYYHtIBCTExMzk2ajBqMagCALACAA&sourceid=chrome&ie=UTF-8#fpstate=ive&vld=cid:acd50391,vid:qz3Q3v84k9Y,st:0
    /// </summary>
    [TestMethod]
    public void Project4VectorOntoAPlaneTest()
    {
        //Input constants
        var vector = new double[3] { 1, 1, 1 };
        var planeNormal = new double[3] { 1, 0, 1 };

        //Run test function
        var projectedVector = VectorService.Normalize(VectorService.ProjectVectorOntoPlane(vector, planeNormal));

        //Expected outputs
        var expectedProjectedVector = new double[3] { 0, 1, 0 };

        //Assert input vs output
        Assert.AreEqual(projectedVector[0], expectedProjectedVector[0], _wolframVectorAccuracy);
        Assert.AreEqual(projectedVector[1], expectedProjectedVector[1], _wolframVectorAccuracy);
        Assert.AreEqual(projectedVector[2], expectedProjectedVector[2], _wolframVectorAccuracy);
    }
    [TestMethod]
    public void Project5VectorOntoAPlaneTest()
    {
        //Input constants
        var vector = new double[3] { 0, 1, 0 };
        var planeNormal = new double[3] { 0, 1, 0 };

        //Run test function
        var projectedVector = VectorService.Normalize(VectorService.ProjectVectorOntoPlane(vector, planeNormal));

        //Expected outputs
        var expectedProjectedVector = new double[3] { double.NaN, double.NaN, double.NaN };

        //Assert input vs output
        Assert.AreEqual(projectedVector[0], expectedProjectedVector[0]);
        Assert.AreEqual(projectedVector[1], expectedProjectedVector[1]);
        Assert.AreEqual(projectedVector[2], expectedProjectedVector[2]);
    }
    [TestMethod]
    public void AngleBetweenTwoVectors1Test()
    {
        //Input constants
        var vector1 = new double[3] { 0.047, 0.66, 0.75 };
        var vector2 = new double[3] { -0.212, 0.977, -0.006 };

        //Run test function
        var angle = VectorService.AngleBetweenTwoVectors(vector1, vector2);

        //Expected outputs
        var expectedAngle = 50.91936D; //In degrees

        //Assert input vs output
        Assert.AreEqual(expectedAngle, angle, _wolframVectorAccuracy);
    }
    [TestMethod]
    public void AngleBetweenTwoVectors2Test()
    {
        //Input constants
        var vector1 = new double[3] { 0.254, -0.384, 0.888 };
        var vector2 = new double[3] { -0.494, -0.272, -0.826 };

        //Run test function
        var angle = VectorService.AngleBetweenTwoVectors(vector1, vector2);

        //Expected outputs
        var expectedAngle = 138.95637D; //In degrees

        //Assert input vs output
        Assert.AreEqual(expectedAngle, angle, _wolframVectorAccuracy);
    }

    [TestMethod]
    public void AngleBetweenTwoVectors3Test()
    {
        //Input constants
        var vector1 = new double[3] { 1, 1, 1 };
        var vector2 = new double[3] { 3, 2, 1 };

        //Run test function
        var angle = VectorService.AngleBetweenTwoVectors(vector1, vector2);

        //Expected outputs
        var expectedAngle = 22.20765429859648748705562002617826867893629403332021656358744100445279054433342996945718978609391666D; //In degrees

        //Assert input vs output
        Assert.AreEqual(expectedAngle, angle, _doubleAccuracy);
    }

    [TestMethod]
    public void AngleBetweenTwoVectors4Test()
    {
        //Input constants
        var vector1 = new double[3] { 1, 0, 0 };
        var vector2 = new double[3] { 1, 1, 0 };

        //Run test function
        var angle = VectorService.AngleBetweenTwoVectors(vector1, vector2);

        //Expected outputs
        var expectedAngle = 45D; //In degrees

        //Assert input vs output
        Assert.AreEqual(expectedAngle, angle, _doubleAccuracy);
    }
}