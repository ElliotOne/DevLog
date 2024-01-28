namespace DevLog.Services.EmailSender
{
    /// <summary>
    /// Default options used by EmailSender.
    /// </summary>
    public class EmailSenderOptions
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
    }
}
