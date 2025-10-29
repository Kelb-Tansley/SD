using SD.Core.Shared.Models;
using SD.Core.Shared.Models.AS;
using SD.Core.Shared.Models.Sans;

namespace SD.Core.Shared.Contracts;
public interface IUlsDesignResults
{
    public List<SansUlsResult>? SansUlsResults { get; set; }
    public List<ASUlsResult>? AsUlsResults { get; set; }

    public void Clear();
    public IEnumerable<UlsResult>? GetUlsResults();
}