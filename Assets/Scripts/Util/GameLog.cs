using UnityEngine;

public static class GameLog
{
    public static void Log(string message)
    {
#if UNITY_EDITOR
        GameLog.Log(message);
#endif
    }
    public static void LogWarning(string message)
    {
#if UNITY_EDITOR
        GameLog.LogWarning(message);
#endif
    }
    public static void LogError(string message)
    {
#if UNITY_EDITOR
        GameLog.LogWarning(message);
#endif
    }
}