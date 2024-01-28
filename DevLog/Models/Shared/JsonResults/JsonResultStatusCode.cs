namespace DevLog.Models.Shared.JsonResults
{
    /// <summary>
    /// Json result status code
    /// </summary>
    public enum JsonResultStatusCode
    {
        Success = 200,
        Unauthorized = 401,
        NotFound = 404,
        ModelStateIsNotValid = 422,
        ServerError = 500
    }
}
