using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;  // Needed for the Unwrap extension method
using Firebase;
using Firebase.Extensions;
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
        firestore = FirebaseFirestore.DefaultInstance;
        firestore.Settings.SslEnabled = true;
        InitFirebase();
    }

    public Firestore(string devHost, string collectionName, string document)
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
        firestore.Settings.Host = "https://localhost:8080";
        this.collectionName = collectionName;
        this.document = document;
        docReference = firestore.Collection(collectionName).Document(document);
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

    public bool GetIsFirebaseEnabled()
    {
        return isFirebaseEnabled;
    }
}