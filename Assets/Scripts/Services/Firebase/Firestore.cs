using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Firestore;

public static class Firestore
{
    private static bool isFirebaseEnabled;
    private static LogLevel logLevel = LogLevel.Verbose;
    private static FirebaseFirestore firestore;
    private static DocumentReference docReference;
    private static string collectionName;
    private static string document;
    private static AppOptions appOptions;
    private static FirebaseApp app;

    public static void Init()
    {
        if (firestore != null)
        {
            return;
        }

        firestore = FirebaseFirestore.DefaultInstance;
        if (Settings.IsFirebaseEmulatorEnabled)
        {
            // In case the config has been cached between scene loads
            if (!firestore.Settings.Host.Contains(Settings.FIRESTORE_HOST))
            {
                firestore.Settings.Host = Settings.FIRESTORE_HOST;
                firestore.Settings.SslEnabled = true;
            }
        }
    }

    // Only during the first login
    public static Task<DocumentSnapshot> GetUserData(string UID)
    {
        //Debug.Log("UID " + UID);
        DocumentReference userData = firestore.Collection(Settings.USER_COLLECTION)?.Document(UID);
        return userData.GetSnapshotAsync();
    }

    public static bool GetIsFirebaseEnabled()
    {
        return isFirebaseEnabled;
    }

    public static Task SaveUserData(Dictionary<string, object> docData)
    {
        if (PlayerData.EmailID == null)
        {
            throw new System.Exception("SaveUserData(). We cannot save an empty user.");
            return null;
        }

        GameLog.Log("Player ID " + PlayerData.EmailID);
        DocumentReference testUser = firestore.Collection(Settings.USER_COLLECTION)?.Document(PlayerData.EmailID);
        Task save = testUser.SetAsync(docData, SetOptions.MergeAll);
        return save;
    }
}