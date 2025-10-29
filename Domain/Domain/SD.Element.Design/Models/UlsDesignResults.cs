using SD.Core.Shared.Constants;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.AS;
using SD.Core.Shared.Models.Sans;

namespace SD.Element.Design.Models;
public class UlsDesignResults(IDesignModel designModel) : IUlsDesignResults
{
    private readonly IDesignModel _designModel = designModel ?? throw new ArgumentNullException(nameof(designModel));

    public List<SansUlsResult>? SansUlsResults { get; set; }

    public List<ASUlsResult>? AsUlsResults { get; set; }

    public void Clear()
    {
        SansUlsResults?.Clear();
        AsUlsResults?.Clear();
    }

    public IEnumerable<UlsResult>? GetUlsResults()
    {
        if (DesignCodes.IsSans(_designModel.DesignCode))
            return SansUlsResults;
        else
            return AsUlsResults;
    }
}