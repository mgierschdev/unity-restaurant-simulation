using System.Threading.Tasks;
using Firebase;

// This load the init code for firebase and auth
public class FirebaseLoad
{
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
            }
            else
            {
                GameLog.LogAll("Error: Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }
}