using UnityEngine;

// Conditional compilation docs: https://docs.unity3d.com/Manual/PlatformDependentCompilation.html

public static class GameLog
{
    public static void Log(string message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }

    public static void Log(Vector3Int message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }

    public static void LogWarning(string message)
    {
#if UNITY_EDITOR
        Debug.LogWarning(message);
#endif
    }
    public static void LogError(string message)
    {
#if UNITY_EDITOR
        Debug.LogWarning(message);
#endif
    }
}