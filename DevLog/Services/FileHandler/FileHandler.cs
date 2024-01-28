using Microsoft.Extensions.Options;

namespace DevLog.Services.FileHandler
{
    /// <summary>
    /// Default implementation for IFileHandler.
    /// </summary>
    public class FileHandler : IFileHandler
    {
        private readonly FileHandlerOptions _fileHandlerOptions;

        public FileHandler(IOptionsSnapshot<FileHandlerOptions> optionsSnapshot)
        {
            _fileHandlerOptions = new FileHandlerOptions()
            {
                WebRootPath = optionsSnapshot.Value.WebRootPath,
                BaseFilesPath = optionsSnapshot.Value.BaseFilesPath,
                PostFilesPath = optionsSnapshot.Value.PostFilesPath,
                ProfileFilesPath = optionsSnapshot.Value.ProfileFilesPath,
                CertificateFilesPath = optionsSnapshot.Value.CertificateFilesPath
            };
        }

        public async Task<bool> Upload(IFormFile formFile, string filePath)
        {
            try
            {
                string absolutePath = GetAbsolutePath(filePath);
                string fileName = Path.GetFileName(filePath);

                if (!Directory.Exists(absolutePath))
                {
                    Directory.CreateDirectory(absolutePath);
                }

                await using var fs = new FileStream(Path.Combine(absolutePath, fileName), FileMode.Create);
                await formFile.CopyToAsync(fs);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(string filePath)
        {
            try
            {
                string absolutePath = GetAbsolutePath(filePath);
                string fileName = Path.GetFileName(filePath);
                string fullFilePath = Path.Combine(absolutePath, fileName);

                if (File.Exists(fullFilePath))
                {
                    File.Delete(fullFilePath);

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public string GetRelativePath(string filename, string fileGuid, FileType fileType)
        {
            string path = $"/{_fileHandlerOptions.BaseFilesPath}";

            path += fileType switch
            {
                FileType.Post => $"/{_fileHandlerOptions.PostFilesPath}",
                FileType.Profile => $"/{_fileHandlerOptions.ProfileFilesPath}",
                FileType.Certificate => $"/{_fileHandlerOptions.CertificateFilesPath}",
                _ => string.Empty
            };

            path += $"/{fileGuid}_{filename}";

            return path;
        }

        private string GetAbsolutePath(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath)!;
            return _fileHandlerOptions.WebRootPath + directoryPath;
        }
    }
}
