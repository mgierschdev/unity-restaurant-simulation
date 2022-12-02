using UnityEngine;
// Conditional compilation docs: https://docs.unity3d.com/Manual/PlatformDependentCompilation.html
public static class GameLog
{
    public static void Log(string message)
    {
#if UNITY_EDITOR
        Debug.Log("GAMELOG UNITY: " + message);
#endif
    }

    public static void Log(int message)
    {
#if UNITY_EDITOR
        Debug.Log("GAMELOG UNITY: " + message);
#endif
    }

    public static void Log(float message)
    {
#if UNITY_EDITOR
        Debug.Log("GAMELOG UNITY: " + message);
#endif
    }

    public static void LogAll(string message)
    {
        Debug.Log("GAMELOG UNITY: " + message);
    }

    public static void Log(Vector3Int message)
    {
#if UNITY_EDITOR
        Debug.Log("GAMELOG UNITY: " + message);
#endif
    }

    public static void LogWarning(string message)
    {
#if UNITY_EDITOR
        Debug.LogWarning("GAMELOG UNITY: " + message);
#endif
    }
    public static void LogError(string message)
    {
#if UNITY_EDITOR
        Debug.LogError(message);
#endif
    }
}