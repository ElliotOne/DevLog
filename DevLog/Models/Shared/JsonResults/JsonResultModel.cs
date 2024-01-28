namespace DevLog.Models.Shared.JsonResults
{
    /// <summary>
    /// Json result model
    /// </summary>
    public class JsonResultModel
    {
        public JsonResultStatusCode StatusCode { get; set; }
        public string? Message { get; set; }
        public string? RedirectUrl { get; set; }
    }
}
