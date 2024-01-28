namespace DevLog.Core.Domain
{
    public class Contact : IEntity
    {
        public int Id { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string EmailOrPhoneNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Ip { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public DateTime LastEditDate { get; set; }
    }
}
