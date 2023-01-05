public static class AnalyticsEvents
{
    public static string CloudCodeSavePlayerData = "CloudCodeSavePlayerData";
    public static string CloudCodeSavePlayerDataResponse = "CloudCodeSavePlayerDataResponse";
}
public enum CloudCodeSavePlayerDataResponse
{
    NEW_PLAYER_SAVED = 1,
    ERROR_WHILE_SAVING = 2,
    PLAYER_LOADED_FROM_CLOUD_SAVE = 3
}