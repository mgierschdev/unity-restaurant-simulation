using System.Threading.Tasks;
using Firebase;
using Firebase.Firestore;

public class Firestore
{
    private bool isFirebaseEnabled;
    private LogLevel logLevel = LogLevel.Verbose;
    private FirebaseFirestore firestore;
    private DocumentReference docReference;
    private string collectionName;
    private string document;
    private AppOptions appOptions;
    private FirebaseApp app;

    public Firestore()
    {
        Init();
    }

    private void Init()
    {
        firestore = FirebaseFirestore.DefaultInstance;
        if (Settings.IsFirebaseEmulatorEnabled)
        {
            if (!firestore.Settings.Host.Contains(Settings.FIRESTORE_HOST))
            {
                firestore.Settings.Host = Settings.FIRESTORE_HOST;
                firestore.Settings.SslEnabled = false;
            }

        }
    }

    // Only during the first login
    public Task<DocumentSnapshot> GetUserData(string UID)
    {
        //Debug.Log("UID " + UID);
        DocumentReference userData = firestore.Collection(Settings.USER_COLLECTION)?.Document(UID);
        return userData.GetSnapshotAsync();
    }

    public bool GetIsFirebaseEnabled()
    {
        return isFirebaseEnabled;
    }
}