namespace SD.Fem.Strand7.Services;

public class Strand7SectionResolver
{
    private const double Tolerance = 1e-3;

    public record SectionMatch(int LibraryID, int ItemID);

    public static SectionMatch? ResolveSectionFromProperty(int modelId, int propNum, bool isBGL, UnitFactor unitFactor)
    {
        // 1. Read geometry from the beam property
        var propGeometry = isBGL
            ? GetPropertyGeometryBGL(modelId, propNum)
            : GetPropertyGeometryBSL(modelId, propNum);

        // 2. Determine library type
        int libraryType = isBGL ? St7.lbSectionGeometry : St7.lbBeamSection;
        int numLibraries = 0;

        // 3. Enumerate libraries
        St7.St7GetNumLibraries(libraryType, ref numLibraries).ThrowIfFails();

        for (int libId = 1; libId <= numLibraries; libId++)
        {
            // 4. Enumerate items in library
            int numItems = 0;
            St7.St7GetNumLibraryItems(libraryType, libId, ref numItems).ThrowIfFails();

            for (int itemId = 1; itemId <= numItems; itemId++)
            {
                // 5. Load library item geometry
                var libGeometry = isBGL
                    ? GetLibraryGeometryBGL(libId, itemId)
                    : GetLibraryGeometryBSL(libId, itemId);

                // 6. Compare geometry
                if (GeometryMatches(propGeometry, libGeometry, unitFactor))
                    return new SectionMatch(libId, itemId);
            }
        }

        return null; // No match found
    }

    // -----------------------------
    // PROPERTY GEOMETRY
    // -----------------------------

    private static double[] GetPropertyGeometryBGL(int modelId, int propNum)
    {
        int shape = 0;
        double[] dims = new double[St7.kMaxBGLDimensions];

        St7.St7GetBeamSectionGeometryBGL(modelId, propNum, ref shape, dims).ThrowIfFails();

        return dims;
    }

    private static double[] GetPropertyGeometryBSL(int modelId, int propNum)
    {
        int sectionType = 0;
        double[] dims = new double[6]; // D1, D2, D3, T1, T2, T3

        St7.St7GetBeamSectionGeometry(modelId, propNum, ref sectionType, dims).ThrowIfFails();

        return dims;
    }

    // -----------------------------
    // LIBRARY GEOMETRY
    // -----------------------------

    private static double[] GetLibraryGeometryBGL(int libraryId, int itemId)
    {
        int shape = 0;
        double[] dims = new double[St7.kMaxBGLDimensions];
        var name = new StringBuilder(256);

        St7.St7GetLibraryBeamSectionGeometryBGL(
            libraryId,
            itemId,
            St7.luMILLIMETRE,
            name,
            name.Length,
            ref shape,
            dims).ThrowIfFails();

        return dims;
    }

    private static double[] GetLibraryGeometryBSL(int libraryId, int itemId)
    {
        int shape = 0;
        double[] dims = new double[6];
        var name = new StringBuilder(256);

        St7.St7GetLibraryBeamSectionPropertyDataBSL(
            libraryId,
            itemId,
            St7.luMILLIMETRE,
            name,
            name.Length,
            ref shape,
            dims).ThrowIfFails();

        return dims;
    }

    // -----------------------------
    // GEOMETRY COMPARISON
    // -----------------------------

    private static bool GeometryMatches(double[] a, double[] b, UnitFactor unitFactor)
    {
        int n = Math.Min(a.Length, b.Length);

        for (int i = 0; i < n; i++)
        {
            if (Math.Abs(a[i] * unitFactor.Length - b[i]) > Tolerance)
                return false;
        }

        return true;
    }
}