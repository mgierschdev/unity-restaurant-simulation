using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class AuthController : MonoBehaviour
{
    public void LoadAuth()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services
            Debug.Log("UNITY: Authenticated." + Social.localUser.userName + " (" + Social.localUser.id);
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }
        else
        {
            Debug.Log("UNITY: Failed to authenticate " + status);
        }
    }
}