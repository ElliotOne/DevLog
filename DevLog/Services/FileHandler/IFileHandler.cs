namespace DevLog.Services.FileHandler
{
    /// <summary>
    /// Represents file handler that is used for file operations
    /// </summary>
    public interface IFileHandler
    {
        Task<bool> Upload(IFormFile formFile, string filePath);
        bool Delete(string filePath);
        string GetRelativePath(string filename, string fileGuid, FileType fileType);
    }
}
