using SD.Core.Shared.Contracts;
using System.Reflection;

namespace SD.Tests.Shared.Strand7;

public static class LocateStrand7TestModel
{
    public static void Initialize(string fileName, IFemModel femModel, IDesignModel designModel, out int modelId)
    {
        var thisLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (thisLocation == null)
            throw new ArgumentNullException(nameof(thisLocation));

        femModel.FileName = Path.Combine(thisLocation, $@"TestFiles\{fileName}");
        designModel.IsLsa = true;
        designModel.IsNla = false;
        modelId = 1;
    }
}
