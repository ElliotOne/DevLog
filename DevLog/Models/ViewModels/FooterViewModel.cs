namespace DevLog.Models.ViewModels
{
    public class FooterViewModel
    {
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? Email1 { get; set; }
        public string? Email2 { get; set; }
        public string? Instagram { get; set; }
        public string? Telegram { get; set; }
        public string? GooglePlus { get; set; }
        public string? FaceBook { get; set; }
        public string? LinkedIn { get; set; }
        public string? Youtube { get; set; }
        public string? GitHub { get; set; }

        public IEnumerable<string> PostTags { get; set; } = new List<string>();
    }
}
