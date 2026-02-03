using Game.Players;
using Util;

namespace Services
{
    /**
     * Problem: Initialize Unity services and authentication.
     * Goal: Sign in anonymously and bootstrap player data.
     * Approach: Use Unity Services initialization and AuthenticationService.
     * Time: O(1) plus network time.
     * Space: O(1).
     */
    public static class UnityAuth
    {
        public static void InitUnityServices()
        {
            GameLog.Log("UnityAuth: offline-only mode; initializing local player data.");
            PlayerData.InitUser();
        }
    }
}
