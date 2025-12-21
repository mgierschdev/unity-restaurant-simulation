// Defined on the Event manager console

namespace Services.Util
{
    /**
     * Problem: Provide names for analytics events.
     * Goal: Centralize event name constants for analytics tracking.
     * Approach: Expose static string fields.
     * Time: O(1) per access.
     * Space: O(1).
     */
    public static class AnalyticsEvents
    {
        public static string CloudCodeGetPlayerData = "CloudCodeGetPlayerData";
        public const string CloudCodeGetPlayerDataResponse = "CloudCodeGetPlayerDataResponse";
    }

    public enum CloudCodeGetPlayerDataResponse
    {
        NewPlayerSaved = 1,
        ErrorWhileSaving = 2,
        PlayerLoadedFromCloudSave = 3
    }
}
