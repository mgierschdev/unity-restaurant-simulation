public static class AnalyticsEvents
{
    public static string SavePlayerData = "SavePlayerData";
    public static string SavePlayerDataResponse = "SavePlayerDataResponse";
}
public enum SavePlayerDataResponse
{
    NEW_PLAYER_SAVED = 1,
    ERROR_WHILE_SAVING = 2,
    PLAYER_LOADED_FROM_CLOUD_SAVE = 3
}