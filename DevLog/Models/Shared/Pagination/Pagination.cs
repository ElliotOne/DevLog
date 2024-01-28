namespace DevLog.Models.Shared.Pagination
{
    public class Pagination
    {
        public Pager Pager { get; set; } = new Pager(default, default, default, default);

        /// <summary>
        /// Query string parameters dictionary
        /// </summary>
        public Dictionary<string, string> Params { get; set; } = new Dictionary<string, string>();
    }
}
