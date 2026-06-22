using SD.Core.Shared.Models;

namespace SD.Element.Design.Interfaces;

public interface IFemFilePathDataService
{
    Task<FemFile?> GetFileByName(string fileName);
    Task<FemFile> GetOrCreateFileByName(string fileName);
}