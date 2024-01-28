namespace DevLog.Services.FileHandler
{
    /// <summary>
    /// Represents file handler options
    /// </summary>
    public class FileHandlerOptions
    {
        /// <summary>
        /// Gets or sets web root path
        /// </summary>
        public string WebRootPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets base file path
        /// </summary>
        public string BaseFilesPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets post files path
        /// </summary>
        public string PostFilesPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets profile files path
        /// </summary>
        public string ProfileFilesPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets certificate files path
        /// </summary>
        public string CertificateFilesPath { get; set; } = string.Empty;
    }
}
