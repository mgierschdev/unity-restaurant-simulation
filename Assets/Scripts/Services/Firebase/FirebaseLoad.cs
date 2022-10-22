using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;

// This load the init code for firebase and auth
public class FirebaseLoad
{
    private bool isUserSignedIn;
    private bool isFirebaseLoaded;
    private FirebaseAuth auth;

    public Task InitAuth()
    {
        auth = FirebaseAuth.DefaultInstance;

        return auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                GameLog.LogAll("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                GameLog.LogAll("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }
            isUserSignedIn = true;
            FirebaseUser newUser = task.Result;
            Debug.LogFormat("GAMELOG UNITY: User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
        });
    }
    
    public Task InitFirebase()
    {
        return FirebaseApp.CheckDependenciesAsync().ContinueWith(checkTask =>
        {
            DependencyStatus status = checkTask.Result;
            if (status != DependencyStatus.Available)
            {
                return FirebaseApp.FixDependenciesAsync().ContinueWith(t =>
                {
                    GameLog.LogAll("Firebase: Loaded.");
                    return FirebaseApp.CheckDependenciesAsync();
                }).Unwrap();
            }
            else
            {
                return checkTask;
            }
        }).Unwrap().ContinueWith(task =>
        {
            DependencyStatus dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // GameLog.Log("Firebase is loaded");
                isFirebaseLoaded = true;
            }
            else
            {
                GameLog.LogAll("Error: Could not resolve all Firebase dependencies: " + dependencyStatus);
                Debug.LogError("Error: Could not resolve all Firebase dependencies: " + dependencyStatus);
                isFirebaseLoaded = false;
            }
        });
    }

    public bool GetIsFirebaseLoaded()
    {
        return isFirebaseLoaded;
    }

    public bool GetIsUserSignedin()
    {
        return isUserSignedIn;
    }

    public FirebaseAuth GetFirebaseAuth()
    {
        return auth;
    }
}