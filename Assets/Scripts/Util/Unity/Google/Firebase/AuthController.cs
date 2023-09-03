// using GooglePlayGames;
// using GooglePlayGames.BasicApi;
// using UnityEngine;
//
//
// // Google play auth
// public class AuthController : MonoBehaviour
// {
//     public void LoadAuth()
//     {
//         PlayGamesPlatform.DebugLogEnabled = true;
//         PlayGamesPlatform.Activate();
//         PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
//     }
//
//     internal void ProcessAuthentication(SignInStatus status)
//     {
//         if (status == SignInStatus.Success)
//         {
//             // Continue with Play Games Services
//             Log("UNITY: Authenticated." + Social.localUser.userName + " (" + Social.localUser.id);
//             PlayGamesPlatform.Instance.ShowAchievementsUI();
//         }
//         else
//         {
//             Log("UNITY: Failed to authenticate " + status);
//         }
//     }
// }