namespace DevLog.Utilities
{
    /// <summary>
    /// Represents GUID utility
    /// </summary>
    public static class GuidUtility
    {
        /// <summary>
        /// Get a new GUID as a string that is safe to be used in URLs
        /// </summary>
        /// <returns>A new safe GUID string</returns>
        public static string NewGuidSafeString()
        {
            return Guid.NewGuid()
                .ToString()
                .Replace("-", "_");
        }
    }
}
