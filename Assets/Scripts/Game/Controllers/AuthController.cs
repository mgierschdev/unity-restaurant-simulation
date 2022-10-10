using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class AuthController : MonoBehaviour
{
    // public void Start()
    // {
    //     // LoadAuth();
    // }

    // void InitializePlayGamesLogin()
    // {
    //     var config = new PlayGamesClientConfiguration.Builder()
    //         // Requests an ID token be generated.  
    //         // This OAuth token can be used to
    //         // identify the player to other services such as Firebase.
    //         .RequestIdToken()
    //         .Build();

    //     PlayGamesPlatform.InitializeInstance(config);
    //     PlayGamesPlatform.DebugLogEnabled = true;
    //     PlayGamesPlatform.Activate();
    // }

    // void LoginGoogle()
    // {
    //     //Social.localUser.Authenticate(ProcessAuthentication);
    // }

    // void OnGooglePlayGamesLogin(bool success)
    // {
    //     if (success)
    //     {
    //         // Call Unity Authentication SDK to sign in or link with Google.
    //         Debug.Log("Login with Google done. IdToken: " + ((PlayGamesLocalUser)Social.localUser).GetIdToken());
    //     }
    //     else
    //     {
    //         Debug.Log("Unsuccessful login");
    //     }
    // }

    public void LoadAuth()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        //PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);

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