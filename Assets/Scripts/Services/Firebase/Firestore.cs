using System.Threading.Tasks;
using Firebase;
using Firebase.Firestore;

public static class Firestore
{
    private static bool isFirebaseEnabled;
    // private static LogLevel logLevel = LogLevel.Verbose;
    private static FirebaseFirestore firestore;
    private static DocumentReference docReference;
    private static string collectionName;
    private static string document;
    private static AppOptions appOptions;
    private static FirebaseApp app;

    public static void Init(bool editor)
    {
        firestore = FirebaseFirestore.DefaultInstance;

        if (editor)
        {
            // In case the config has been cached between scene loads
            if (!firestore.Settings.Host.Contains(Settings.FIRESTORE_HOST))
            {
                firestore.Settings.Host = Settings.FIRESTORE_HOST;
                firestore.Settings.SslEnabled = false;
            }
        }
    }

    // Only during the first login
    public static Task<DocumentSnapshot> GetUserData(string UID)
    {
        DocumentReference userData = null;

        if (Settings.IsFirebaseEmulatorEnabled)
        {
            userData = firestore.Collection(Settings.USER_PRED_PROD_COLLECTION).Document(UID);
        }
        else
        {
            userData = firestore.Collection(Settings.USER_PRED_PROD_COLLECTION).Document(UID);
        }

        return userData.GetSnapshotAsync() ?? null;
    }

    public static Task SaveObject(FirebaseGameUser user)
    {
        DocumentReference testUser = firestore.Collection(Settings.USER_PRED_PROD_COLLECTION)?.Document(user.FIREBASE_AUTH_ID);
        return testUser.SetAsync(user, SetOptions.MergeAll);
    }
}