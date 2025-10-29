using SD.Core.Shared.Enum;
using SD.Core.Shared.Extensions;

namespace SD.Element.Design.Services;
public static class VectorService
{
    /// <summary>
    /// Returns a unit vector in the direction provided.
    /// </summary>
    /// <param name="axis"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static double[] GetUnitVectorByAxis(GlobalAxis axis)
    {
        return axis switch
        {
            GlobalAxis.X => [1, 0, 0],
            GlobalAxis.Y => [0, 1, 0],
            GlobalAxis.Z => [0, 0, 1],
            GlobalAxis.NotAlligned => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };
    }

    /// <summary>
    /// Returns the negative of the input vector
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static double[] NegateVector(double[] vector)
    {
        var negativeVector = new double[vector.Length];
        for (int i = 0; i < vector.Length; i++)
            negativeVector[i] = -1 * vector[i];

        return negativeVector;
    }
    /// <summary>
    /// Returns the orientation between two vectors.
    /// </summary>
    /// <param name="vector1"></param>
    /// <param name="vector2"></param>
    /// <returns></returns>
    public static double VectorOrientation(double[] vector1, double[] vector2) => DotProduct(vector1, vector2) > 0 ? 1D : -1D;

    /// <summary>
    /// Determines the vector which represents a vector projection onto a plane. 
    /// The plane normal vector is the vector perpendicular to the plane.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="planeNormal"></param>
    /// <returns></returns>
    public static double[] ProjectVectorOntoPlane(double[] vector, double[] planeNormal)
    {
        // Normalize the plane normal vector
        var normalMagnitude = Magnitude(planeNormal);
        var normalizedPlaneNormal = Normalize(planeNormal);

        // Calculate the dot product of the vector and the plane normal
        var dotProduct = DotProduct(vector, normalizedPlaneNormal);

        // Subtract the scaled normal from the original vector
        return vector.Zip(normalizedPlaneNormal, (v, n) => v - dotProduct * n).ToArray();
    }

    /// <summary>
    /// Normalizes the vector such that the magnitude of the normalized vector is 1.
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static double[] Normalize(double[] vector)
    {
        var magnitude = Magnitude(vector);
        return vector.Select(coord => coord / magnitude).ToArray();
    }

    /// <summary>
    /// Angle between two vectors. Answer in degrees.
    /// </summary>
    public static double AngleBetweenTwoVectors(double[] vector1, double[] vector2)
    {
        // Calculate the dot product of the two vectors
        var dotProduct = DotProduct(vector1, vector2);

        // Calculate the magnitude of the two vectors
        var magnitudeVector1 = Magnitude(vector1);
        var magnitudeVector2 = Magnitude(vector2);

        if (dotProduct == magnitudeVector1 * magnitudeVector2)
            return 0; //This proves that vectors are parallel

        // Calculate the angle in radians
        var angleRatio = dotProduct / (magnitudeVector1 * magnitudeVector2);
        if (angleRatio < 0)
            angleRatio = Math.Max(-1, angleRatio);
        else if (angleRatio > 0)
            angleRatio = Math.Min(1, angleRatio);

        var angleRadians = Math.Acos(angleRatio);

        return angleRadians.RadiansToDegrees();
    }

    /// <summary>
    /// Checks that the angle between two vectors is not colinear, by an inputted angle limit. All angles in degrees.
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    public static bool AngleIsOutsideColinearLimit(double angle, double limit)
    {
        var absAngle = Math.Abs(angle);
        if (absAngle > 90 && absAngle <= 180)
            return 180 - absAngle > limit;
        else if (absAngle <= 90)
            return absAngle > limit;
        else
            return false;
    }

    private static double Magnitude(double[] vector) => Math.Sqrt(vector.Sum(coord => coord * coord));

    private static double DotProduct(double[] vector1, double[] vector2) => vector1.Zip(vector2, (v1, v2) => v1 * v2).Sum();
}
