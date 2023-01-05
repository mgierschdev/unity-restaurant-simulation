// Defined on the Event manager console
public static class AnalyticsEvents
{
    public static string CloudCodeGetPlayerData = "CloudCodeGetPlayerData";
    public static string CloudCodeGetPlayerDataResponse = "CloudCodeGetPlayerDataResponse";
}
public enum CloudCodeGetPlayerDataResponse
{
    NEW_PLAYER_SAVED = 1,
    ERROR_WHILE_SAVING = 2,
    PLAYER_LOADED_FROM_CLOUD_SAVE = 3
}