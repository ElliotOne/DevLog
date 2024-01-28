namespace DevLog.Utilities
{
    /// <summary>
    /// Represents DateTime utility
    /// </summary>
    public static class DateTimeUtility
    {
        /// <summary>
        /// Convert a datetime to a user friendly representation; HH:mm:ss yyyy/MM/dd
        /// </summary>
        /// <param name="dateTime">DateTime</param>
        /// <returns>DateTime in HH:mm:ss yyyy/MM/dd format</returns>
        public static string ToFriendlyFullDateTime(this DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss yyyy/MM/dd");
        }
    }
}
