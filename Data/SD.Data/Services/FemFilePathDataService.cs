using SD.Data.Entities;
using SD.Data.Interfaces;

namespace SD.Data.Services;

public class FemFilePathDataService(IUnitOfWork unitOfWork) : IFemFilePathDataService
{
    private readonly IRepository<FemFileEntity> _femFileRepo = unitOfWork.GetRepository<FemFileEntity>();

    public async Task<FemFile?> GetFileByName(string fileName)
    {
        // First determine if the file can be found in the database, by full path.
        var file = await _femFileRepo.FirstOrDefault(f => f.FileName == fileName);
        if (file is not null)
            return new FemFile { FileId = file.StableId, FemModelFilePath = file.FileName, ExactMatchFound = true };

        // The file may have been moved, so it cannot be found by the full path. Use file name only to find the file in the database.
        var fileNameOnly = Path.GetFileName(fileName);
        file = await _femFileRepo.FirstOrDefault(f => f.FileNameOnly == fileNameOnly);
        if (file is not null)
            return new FemFile { FileId = file.StableId, FemModelFilePath = file.FileName, ExactMatchFound = false };

        return null;
    }

    public async Task<FemFile> GetOrCreateFileByName(string fileName)
    {
        var file = await GetFileByName(fileName);
        if (file is not null)
            return file;

        var newFile = new FemFileEntity { FileName = fileName, FileNameOnly = Path.GetFileName(fileName) };
        await _femFileRepo.AddAsync(newFile);
        await unitOfWork.Commit();

        return new FemFile { FileId = newFile.StableId, FemModelFilePath = newFile.FileName, ExactMatchFound = true };
    }
}