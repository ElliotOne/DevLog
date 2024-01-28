namespace DevLog.Core.Domain
{
    public class Setting : IEntity
    {
        public int Id { get; set; }
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? Email1 { get; set; }
        public string? Email2 { get; set; }
        public string? Instagram { get; set; }
        public string? Telegram { get; set; }
        public string? GooglePlus { get; set; }
        public string? FaceBook { get; set; }
        public string? LinkedIn { get; set; }
        public string? YouTube { get; set; }
        public string? GitHub { get; set; }
        public string? WalletName1 { get; set; }
        public string? WalletAddress1 { get; set; }
        public string? WalletName2 { get; set; }
        public string? WalletAddress2 { get; set; }
        public string? WalletName3 { get; set; }
        public string? WalletAddress3 { get; set; }
        public string? WalletName4 { get; set; }
        public string? WalletAddress4 { get; set; }
    }
}
