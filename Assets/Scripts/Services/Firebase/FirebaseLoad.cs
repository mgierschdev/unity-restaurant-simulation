using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;

// This load the init code for firebase and the init auth
public class FirebaseLoad
{
    private bool isUserSignedIn;

    private Task InitAuthAnonymosly()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        return auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }
            isUserSignedIn = true;
            FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
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
                GameLog.Log("Firebase is loaded");
                isFirebaseEnabled = true;
            }
            else
            {
                GameLog.LogError("Error: Could not resolve all Firebase dependencies: " + dependencyStatus);
                isFirebaseEnabled = false;
            }
        });
    }
}