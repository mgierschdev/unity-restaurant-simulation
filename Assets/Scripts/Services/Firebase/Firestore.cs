using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Firestore;
using UnityEngine;
using DependencyStatus = Firebase.DependencyStatus;

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
        // firestore = FirebaseFirestore.DefaultInstance;
        // firestore.Settings.SslEnabled = true;
        // InitFirebase();
    }

    public FirebaseFirestore GetFirestoreEmulatorInstance()
    {
        
        AppOptions options = new AppOptions
        {
            ApiKey = "<API_KEY>",
            AppId = "<GOOGLE_APP_ID>",
            ProjectId = "fir-unitygamebussbackend"
        };
        // app.Options.ProjectId = "fir-unitygamebussbackend";
        // app.Options.DatabaseUrl = new Uri("http://localhost:8080/");
        FirebaseApp app = FirebaseApp.Create(options, "Secondary");
        if(app == null){
            Debug.Log("FirebaseApp null");
        }

        firestore = FirebaseFirestore.GetInstance(app);
        Debug.Log(firestore.App+" "+firestore.ToString() +" "+ app.ToString());
        firestore.Settings.SslEnabled = false;
        firestore.Settings.Host = "localhost:8080";
        return firestore;
    }

    public Task SaveDictionary(Dictionary<string, object> dictionary)
    {
        Debug.Log("Saving dic ");

        try
        {
            return docReference.SetAsync(dictionary);
        }
        catch (FirebaseException e)
        {
            GameLog.LogError("Exception raised: Firestore/SaveDictionary " + e.ToString());
            return null;
        }
    }

    public bool GetIsFirebaseEnabled()
    {
        return isFirebaseEnabled;
    }
}