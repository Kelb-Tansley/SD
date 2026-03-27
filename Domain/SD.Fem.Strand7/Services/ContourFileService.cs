namespace SD.Fem.Strand7.Services;

public class ContourFileService(IAppSettings appSettings) : IContourFileService
{
    public async Task<string> GenerateDesignableBeamsContourFile(List<Beam> beams)
    {
        const string fileName = "Designable Beams Contour";
        const string fileTitle = "Designable Beams (True = 1 & False = 0)\n";

        var fileRows = new StringBuilder();
        foreach (var beam in beams)
        {
            var designable = beam.CanDesign() ? 1 : 0;
            fileRows.AppendLine($"{beam.Number} {designable} {designable}");
        }

        return await GenerateBeamContourFile(fileName, fileTitle, fileRows);
    }
    public async Task<string> GenerateL1ContourFile(List<Beam> beams, double lengthFactor)
    {
        const string fileName = "Design Length 1 Contour";
        const string fileTitle = "Design Length (Minor axis)\n";

        var fileRows = new StringBuilder();
        foreach (var beam in beams)
            fileRows.AppendLine($"{beam.Number} {beam.BeamChain.L1 / lengthFactor} {beam.BeamChain.L1 / lengthFactor}");

        return await GenerateBeamContourFile(fileName, fileTitle, fileRows);
    }
    public async Task<string> GenerateL2ContourFile(List<Beam> beams, double lengthFactor)
    {
        const string fileName = "Design Length 2 Contour";
        const string fileTitle = "Design Length (Major Axis)\n";

        var fileRows = new StringBuilder();
        foreach (var beam in beams)
            fileRows.AppendLine($"{beam.Number} {beam.BeamChain.L2 / lengthFactor} {beam.BeamChain.L2 / lengthFactor}");

        return await GenerateBeamContourFile(fileName, fileTitle, fileRows);
    }

    public async Task<string> GenerateLzContourFile(List<Beam> beams, double lengthFactor)
    {
        const string fileName = "Design Length Z Contour";
        const string fileTitle = "Design Length Z (Torsional Axis)\n";

        var fileRows = new StringBuilder();
        foreach (var beam in beams)
            fileRows.AppendLine($"{beam.Number} {beam.BeamChain.Lz / lengthFactor} {beam.BeamChain.Lz / lengthFactor}");

        return await GenerateBeamContourFile(fileName, fileTitle, fileRows);
    }
    public async Task<string> GenerateLeTopContourFile(List<Beam> beams, double lengthFactor)
    {
        const string fileName = "Design Length E Top Contour";
        const string fileTitle = "Design Length E Top (Bending Axis)\n";

        var fileRows = new StringBuilder();
        foreach (var beam in beams)
            fileRows.AppendLine($"{beam.Number} {beam.BeamChain.LeTop / lengthFactor} {beam.BeamChain.LeTop / lengthFactor}");

        return await GenerateBeamContourFile(fileName, fileTitle, fileRows);
    }
    public async Task<string> GenerateLeBottomContourFile(List<Beam> beams, double lengthFactor)
    {
        const string fileName = "Design Length E Bottom Contour";
        const string fileTitle = "Design Length E Bottom (Bending Axis)\n";

        var fileRows = new StringBuilder();
        foreach (var beam in beams)
            fileRows.AppendLine($"{beam.Number} {beam.BeamChain.LeBottom / lengthFactor} {beam.BeamChain.LeBottom / lengthFactor}");

        return await GenerateBeamContourFile(fileName, fileTitle, fileRows);
    }
    public async Task<string> GenerateL1R1ContourFile(List<Beam> beams, double lengthFactor)
    {
        const string fileName = "Design Slenderness 1 Contour";
        const string fileTitle = "Design Slenderness (Minor Axis)\n";

        var fileRows = new StringBuilder();
        foreach (var beam in beams)
        {
            var slenderness = beam.BeamChain.K1 * beam.BeamChain.L1;
            if (beam.Section.SectionType == SectionType.Angle)
                slenderness /= beam.Section.RMinor;
            else
                slenderness /= beam.Section.RMajor;

            fileRows.AppendLine($"{beam.Number} {slenderness} {slenderness}");
        }

        return await GenerateBeamContourFile(fileName, fileTitle, fileRows);
    }
    public async Task<string> GenerateL2R2ContourFile(List<Beam> beams, double lengthFactor)
    {
        const string fileName = "Design Slenderness 2 Contour";
        const string fileTitle = "Design Slenderness (Major Axis)\n";

        var fileRows = new StringBuilder();
        foreach (var beam in beams)
        {
            var slenderness = beam.BeamChain.K2 * beam.BeamChain.L2;
            if (beam.Section.SectionType == SectionType.Angle)
                slenderness /= beam.Section.RMajor;
            else
                slenderness /= beam.Section.RMinor;

            fileRows.AppendLine($"{beam.Number} {slenderness} {slenderness}");
        }

        return await GenerateBeamContourFile(fileName, fileTitle, fileRows);
    }
    public async Task<string> GenerateResultsContourFile(List<UlsResultPeak> results)
    {
        const string fileName = "ULS Results Contour";
        var firstResult = results.First();
        var fileTitle = firstResult.DesignCode == DesignCode.SANS ? $"ULS Utilization - {firstResult.PeakType} - (%)\n" : "ULS Utilisation (%)\n";

        var fileRows = new StringBuilder();
        foreach (var result in results)
            fileRows.AppendLine($"{result.BeamId} {(result.Utilization * 100).Cut()} {(result.Utilization * 100).Cut()}");

        return await GenerateBeamContourFile(fileName, fileTitle, fileRows);
    }

    public async Task<string> GenerateSlsResultsContourFile(List<DeflectionResult> results, DeflectionAxis deflectionAxis)
    {
        const string fileName = "SLS Results Contour";
        var fileTitle = $"Beam SLS Deflection Ratio in the D{deflectionAxis} axis";

        var fileRows = new StringBuilder();
        foreach (var result in results)
            fileRows.AppendLine($"{result.BeamId} {result.DeflectionRatio} {result.DeflectionRatio}");

        return await GenerateBeamContourFile(fileName, fileTitle, fileRows);
    }

    private async Task<string> GenerateBeamContourFile(string fileName, string fileTitle, StringBuilder rows)
    {
        try
        {
            if (!Directory.Exists(appSettings.ContourLocation))  // if it doesn't exist, create
                Directory.CreateDirectory(appSettings.ContourLocation);

            var fullFileName = Path.Join(appSettings.ContourLocation, $@"{fileName}.txt");
            if (File.Exists(fullFileName))
                File.Delete(fullFileName);

            using (var fs = new FileStream(fullFileName, FileMode.CreateNew))
            {
                using var writer = new StreamWriter(fs);
                await writer.WriteAsync($"TITLE {fileTitle}");
                await writer.WriteAsync(rows);

                await writer.FlushAsync();
                writer.Close();
            }

            return fullFileName;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<string> GenerateK1ContourFile(List<Beam> beams)
    {
        const string fileName = "Design K 1 Contour";
        const string fileTitle = "Design K 1 (Minor axis)\n";

        var fileRows = new StringBuilder();
        foreach (var beam in beams)
            fileRows.AppendLine($"{beam.Number} {beam.BeamChain.K1} {beam.BeamChain.K1}");

        return await GenerateBeamContourFile(fileName, fileTitle, fileRows);
    }

    public async Task<string> GenerateK2ContourFile(List<Beam> beams)
    {
        const string fileName = "Design K 2 Contour";
        const string fileTitle = "Design K 2 (Minor axis)\n";

        var fileRows = new StringBuilder();
        foreach (var beam in beams)
            fileRows.AppendLine($"{beam.Number} {beam.BeamChain.K2} {beam.BeamChain.K2}");

        return await GenerateBeamContourFile(fileName, fileTitle, fileRows);
    }

    public async Task<string> GenerateKzContourFile(List<Beam> beams)
    {
        const string fileName = "Design Kz Contour";
        const string fileTitle = "Design Kz (Torsional axis)\n";

        var fileRows = new StringBuilder();
        foreach (var beam in beams)
            fileRows.AppendLine($"{beam.Number} {beam.BeamChain.Kz} {beam.BeamChain.Kz}");

        return await GenerateBeamContourFile(fileName, fileTitle, fileRows);
    }

    public async Task<string> GenerateKeTopContourFile(List<Beam> beams)
    {
        const string fileName = "Design Ke Top Contour";
        const string fileTitle = "Design Ke (Top bending axis)\n";

        var fileRows = new StringBuilder();
        foreach (var beam in beams)
            fileRows.AppendLine($"{beam.Number} {beam.BeamChain.KeTop} {beam.BeamChain.KeTop}");

        return await GenerateBeamContourFile(fileName, fileTitle, fileRows);
    }

    public async Task<string> GenerateKeBottomContourFile(List<Beam> beams)
    {
        const string fileName = "Design Ke Bottom Contour";
        const string fileTitle = "Design Ke (Bottom bending axis)\n";

        var fileRows = new StringBuilder();
        foreach (var beam in beams)
            fileRows.AppendLine($"{beam.Number} {beam.BeamChain.KeBottom} {beam.BeamChain.KeBottom}");

        return await GenerateBeamContourFile(fileName, fileTitle, fileRows);
    }
}