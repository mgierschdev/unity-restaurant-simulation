// Defined on the Event manager console

namespace Services.Util
{
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